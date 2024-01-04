using System;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;

namespace ConsoleApp1.Secrets
{
    public interface ISecretCache
    {
        Task<SecretModel> GetOrAddSecretValue(string store, string key, Func<string, Task<KeyVaultSecret>> fetchBundle);
    }
}