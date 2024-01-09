namespace ConsoleApplication.Startup;

using Microsoft.Extensions.Configuration;

public static class StartupExtensions
{
  public static IConfigurationBuilder AddApplicationConsoleConfigurations(this IConfigurationBuilder configBuilder)
  {
    configBuilder.AddJsonFile("Startup/appsettings.json", optional: false, reloadOnChange: true);

    return configBuilder;
  }
}