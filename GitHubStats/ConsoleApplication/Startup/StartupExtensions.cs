namespace ConsoleApplication.Startup;

using Microsoft.Extensions.Configuration;

public static class StartupExtensions
{
  public static string GetApplicationPath(string? file = null, string? folder = null)
  {
    // Base directory path
    var applicationPath = Directory.GetCurrentDirectory();

    // Set the path separator
    static string setPathSeparator(string path) => $"/{path}";

    // Include project folder if running via GitHub actions (not locally)
#if ACTION
  applicationPath += setPathSeparator("ConsoleApplication");
#endif

    // Include path/folders
    applicationPath += string.IsNullOrEmpty(folder) == false ? setPathSeparator(folder) : string.Empty;
    applicationPath += string.IsNullOrEmpty(file) == false ? setPathSeparator(file) : string.Empty;

    return applicationPath;
  }

  public static IConfigurationBuilder AddApplicationConsoleConfigurations(this IConfigurationBuilder configBuilder)
  {
    // Configure the appsettings.json
    var appSettingsPath = GetApplicationPath(file: "appsettings.json", folder: "Startup");
    configBuilder.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);

    return configBuilder;
  }
}