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

    using System;
using System.Collections.Generic;
using System.Linq;

class Program
    {
        static void Main()
        {
            // Assuming you have a dictionary
            Dictionary<string, string> inputDictionary = new Dictionary<string, string>
        {
            { "Configom", "0:AT,1:AD,2:dd" },
            // Add more entries if needed
        };

            string targetValue = "0"; // Change this to the desired value

            var matchingEntry = inputDictionary
                .Where(entry => entry.Value.Split(',')
                                         .Select(part => part.Split(':'))
                                         .Any(keyValue => keyValue.Length == 2 && keyValue[0] == targetValue))
                .FirstOrDefault();

            if (matchingEntry.Key != null)
            {
                string correspondingValue = matchingEntry.Value
                    .Split(',')
                    .Select(part => part.Split(':'))
                    .FirstOrDefault(keyValue => keyValue.Length == 2 && keyValue[0] == targetValue)?
                    .ElementAtOrDefault(1);

                Console.WriteLine($"Key associated with exact value '{targetValue}': {matchingEntry.Key}");
                Console.WriteLine($"Corresponding value: {correspondingValue ?? "Not found"}");
            }
            else
            {
                Console.WriteLine($"Exact value '{targetValue}' not found in the dictionary");
            }
        }
    }



}
