namespace ConsoleApplication.Startup;

using Microsoft.Extensions.Configuration;

public static class StartupExtensions
{
  public static string GetApplicationPath(string? file = null, string? folders = null)
  {
    // Base directory path
    var applicationPath = Directory.GetCurrentDirectory();

    // Include project folder if running via GitHub actions (not locally)
#if ACTION
  applicationPath += "\\ConsoleApplication";
#endif

    // Include path/folders
    applicationPath = string.IsNullOrEmpty(folders) == false ? Path.Combine(applicationPath, folders) : applicationPath;
    applicationPath = string.IsNullOrEmpty(file) == false ? Path.Combine(applicationPath, file) : applicationPath;

    return applicationPath;
  }

  public static IConfigurationBuilder AddApplicationConsoleConfigurations(this IConfigurationBuilder configBuilder)
  {
    // Configure the appsettings.json
    var appSettingsPath = GetApplicationPath(file: "appsettings.json", folders: "Startup");
    configBuilder.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);

    return configBuilder;
  }
}