namespace ConsoleApplication.Startup;

using Microsoft.Extensions.Configuration;

public static class StartupExtensions
{
  public static IConfigurationBuilder AddApplicationConsoleConfigurations(this IConfigurationBuilder configBuilder)
  {
    // Configure the appsettings.json
    var currentDirectory = Directory.GetCurrentDirectory();
    var appSettingsPath = Path.Combine(currentDirectory, "Startup/appsettings.json");
    configBuilder.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);

    return configBuilder;
  }
}