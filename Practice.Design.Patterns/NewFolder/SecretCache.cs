using Kmd.Logic.Thor.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;

namespace ConsoleApp1.Secrets
{
    internal sealed class SecretCache : IDisposable, ISecretCache
    {
        public static readonly TimeSpan CheckInterval = new TimeSpan(0, 1, 00);
        public static readonly TimeSpan FlushInterval = new TimeSpan(0, 5, 00);

        private readonly SemaphoreSlim _slimLock = new SemaphoreSlim(1, 1);
        private readonly IClock _clock;
        private readonly Dictionary<string, SecretValueDetails> _secretsByKey = new Dictionary<string, SecretValueDetails>();
        private readonly Dictionary<string, SecretValueDetails> _secretsByValue = new Dictionary<string, SecretValueDetails>();
        private DateTimeOffset _lastFlush;

        public SecretCache(IClock clock)
        {
            _clock = clock;
            _lastFlush = clock.UtcNow;
        }

        public async Task<SecretModel> GetOrAddSecretValue(string store, string key, Func<string, Task<KeyVaultSecret>> fetchBundle)
        {
            var cacheKey = GenerateKey(store, key);

            await _slimLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_secretsByKey.TryGetValue(cacheKey, out SecretValueDetails details)
                    && details.BundleIsRecent())
                {
                    return details.GetSecretModel();
                }

                RemoveOldSecrets();

                KeyVaultSecret bundle;
                try
                {
                    bundle = await fetchBundle(key).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to access secrect {Key}, {Message}", key, ex.Message);
                    throw new CertificateCacheException("Unable to access the secret");
                }

                if (bundle == null) throw new CertificateCacheException($"Missing secret {key}");

                return Refresh(cacheKey, details, bundle);
            }
            finally
            {
                _slimLock.Release();
            }
        }

        private SecretModel Refresh(string key, SecretValueDetails exitingDetails, KeyVaultSecret secret)
        {
            if (exitingDetails != null)
            {
                if (exitingDetails.KeyVaultSecretMatches(secret))
                {
                    // The secret still matches
                    return exitingDetails.GetSecretModel();
                }

                // The bundle for this key no longer aligns
                _secretsByKey.Remove(key);

                if (exitingDetails.ReleaseKey(key))
                {
                    // We are done with this secret entirely
                    _secretsByValue.Remove(exitingDetails.KeyVaultSecret.Value);
                }
            }

            SecretValueDetails details;
            if (_secretsByValue.TryGetValue(secret.Value, out details))
            {
                // We have the same secret registered under another key - share it
                details.AddKey(key);
                _secretsByKey.Add(key, details);
            }
            else
            {
                // This secret is totally new
                details = new SecretValueDetails(_clock, key, secret);

                _secretsByKey.Add(key, details);
                _secretsByValue.Add(details.KeyVaultSecret.Value, details);
            }

            return details.GetSecretModel();
        }

        private void RemoveOldSecrets()
        {
            var old = _clock.UtcNow - FlushInterval;

            if (_lastFlush > old) return;

            Flush(old);
        }

        private void Flush(DateTimeOffset maxLastAccessed)
        {
            var oldDetails = _secretsByValue.Values.Where(x => x.LastAccessed <= maxLastAccessed).ToList();

            foreach (var details in oldDetails)
            {
                _secretsByValue.Remove(details.KeyVaultSecret.Value);

                foreach (var key in details.Keys)
                {
                    _secretsByKey.Remove(key);
                }
            }

            _lastFlush = _clock.UtcNow;
        }

        private static string GenerateKey(string store, string key)
        {
            return $"{key}@{store}";
        }

        public void Dispose()
        {
            _slimLock.Dispose();
        }

        private sealed class SecretValueDetails
        {
            private readonly List<string> _keys;
            private readonly IClock _clock;

            public KeyVaultSecret KeyVaultSecret { get; }
            public DateTimeOffset LastChecked { get; private set; }
            public DateTimeOffset LastAccessed { get; private set; }
            public IEnumerable<string> Keys => _keys;

            public SecretValueDetails(IClock clock, string key, KeyVaultSecret keyVaultSecret)
            {
                _keys = new List<string> { key };
                _clock = clock;
                KeyVaultSecret = keyVaultSecret;
                LastChecked = _clock.UtcNow;
                LastAccessed = LastChecked;
            }

            public SecretModel GetSecretModel()
            {
                return new SecretModel(KeyVaultSecret.Name,
                    KeyVaultSecret.Value,
                    KeyVaultSecret.Id.ToString());
            }

            public bool BundleIsRecent()
            {
                LastAccessed = _clock.UtcNow;
                return _clock.UtcNow - LastChecked < CheckInterval;
            }

            public bool KeyVaultSecretMatches(KeyVaultSecret keyVaultSecret)
            {
                LastAccessed = _clock.UtcNow;

                if (KeyVaultSecret.Value == keyVaultSecret.Value)
                {
                    LastChecked = _clock.UtcNow;
                    return true;
                }

                return false;
            }

            public void AddKey(string key)
            {
                _keys.Add(key);
            }

            public bool ReleaseKey(string key)
            {
                _keys.Remove(key);
                return _keys.Count == 0;
            }
        }
    }
}