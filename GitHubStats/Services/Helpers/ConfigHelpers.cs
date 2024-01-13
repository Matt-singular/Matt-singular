namespace Services.Helpers;

using Microsoft.Extensions.Configuration;

public static class ConfigHelpers
{
  /// <summary>
  /// Not at all necessary, but here to centralise appsettings-config-based logic
  /// </summary>
  /// <param name="configuration">The injected configuration service that was set on application startup</param>
  /// <param name="key">The value of the config entry to be pulled</param>
  /// <returns></returns>
  public static string? GetConfigValue(this IConfiguration configuration, string? key)
  {
    if (string.IsNullOrEmpty(key))
    {
      // Short-Circuit if no key was provided
      return null;
    }

    // Otherwise return the configured value that was found
    var configValue = configuration[key];

    return configValue;
  }

  /// <summary>
  /// Localised ConfigHelper constants
  /// </summary>
  public static class Constants
  {
    public const string AccessTokenKey = "AccessToken";
    public const string UserOrganisationRepositories = "UserOrganisationRepositories";
  }
}
