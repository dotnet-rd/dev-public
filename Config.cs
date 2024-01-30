
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
        string targetValue = "0"; // Change this to the desired value

        var matchingKeys = inputDictionary
            .Where(entry => entry.Value.Split(',')
                                     .Select(part => part.Split(':'))
                                     .Any(keyValue => keyValue.Length == 2 && keyValue[0] == targetValue))
            .Select(entry => entry.Key)
            .ToList();

        if (matchingKeys.Any())
        {
            Console.WriteLine($"Keys associated with exact value '{targetValue}': {string.Join(", ", matchingKeys)}");
        }
        else
        {
            Console.WriteLine($"Exact value '{targetValue}' not found in the dictionary");
        }
    }
}
