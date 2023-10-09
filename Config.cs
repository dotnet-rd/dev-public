
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