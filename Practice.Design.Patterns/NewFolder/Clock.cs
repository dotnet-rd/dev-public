using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Thor.Framework
{
    /// <summary>
    /// Prefer this instead of directly using <see cref="DateTimeOffset.UtcNow"/>
    /// because this can more easily be mocked for testing purposes.
    /// </summary>
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
    }

    internal class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }


    [Serializable]
    public class CertificateCacheException : Exception
    {
        public CertificateCacheException()
        {
        }

        public CertificateCacheException(string message) : base(message)
        {
        }

        public CertificateCacheException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CertificateCacheException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
