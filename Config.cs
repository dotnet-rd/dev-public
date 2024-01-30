
public static TConfiguration GetSection<TConfiguration>(this IConfiguration configuration, string key, bool optional = false) where TConfiguration : class, new()
{
    var configurationSection = configuration.GetSection(key).Get<TConfiguration>();
    if (configurationSection == null)
    {
        if (optional)
        {
            configurationSection = new TConfiguration();
        }
        else
        {
            throw new ConfigurationErrorException($"The configuration section '{key}' is missing or incorrect");
        }
    }

    return configurationSection;
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
            { "Configom", "0:AT,1:AD,2:dd" }
            // Add more entries if needed
        };

        string targetKey = "Configom"; // Change this to the desired key
        int targetIndex = 1; // Change this to the desired index

        if (inputDictionary.TryGetValue(targetKey, out string value))
        {
            var result = value
                .Split(',')
                .Select(part => part.Split(':'))
                .Where(keyValue => keyValue.Length == 2 && int.TryParse(keyValue[0], out int index) && index == targetIndex)
                .Select(keyValue => keyValue[1])
                .FirstOrDefault();

            if (result != null)
            {
                Console.WriteLine($"Value at index {targetIndex} for key '{targetKey}': {result}");
            }
            else
            {
                Console.WriteLine($"Index {targetIndex} not found in the value associated with key '{targetKey}'");
            }
        }
        else
        {
            Console.WriteLine($"Key '{targetKey}' not found in the dictionary");
        }
    }
}
