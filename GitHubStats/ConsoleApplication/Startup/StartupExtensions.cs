namespace ConsoleApplication.Startup;

using Microsoft.Extensions.Configuration;
using Services.Helpers;

public static class StartupExtensions
{
  /// <summary>
  /// Sets up the application's config files (and user secret overrides) at Startup
  /// </summary>
  /// <param name="configBuilder"></param>
  /// <returns>the configured IConfigurationBuilder</returns>
  public static IConfigurationBuilder AddApplicationConsoleConfigurations(this IConfigurationBuilder configBuilder)
  {
    // Configure the appsettings.json
    var appSettingsPath = FileHelpers.GetApplicationPath(file: Constants.AppSettingsFile, folders: Constants.AppSettingsFolder);
    configBuilder.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);

    // Configure user secret overrides
    configBuilder.AddUserSecrets<Program>();

    // Add environment varialbe overrides
    configBuilder.AddEnvironmentVariables();

    return configBuilder;
  }

  /// <summary>
  /// Localised StartupExtension constants
  /// </summary>
  public static class Constants
  {
    public const string AppSettingsFile = "appsettings.json";
    public const string AppSettingsFolder = "Startup";
  }
}