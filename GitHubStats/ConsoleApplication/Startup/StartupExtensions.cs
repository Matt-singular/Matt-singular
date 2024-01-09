namespace ConsoleApplication.Startup;

using Microsoft.Extensions.Configuration;

public static class StartupExtensions
{
  public static string GetApplicationPath(string? file = null, string? folders = null)
  {
    // Base directory path
    var currentDirectory = Directory.GetCurrentDirectory();

    // Include project folder if running via GitHub actions (not locally)
#if ACTION
    currentDirectory += "\\ConsoleApplication";
#endif

    // Include path/folders
    var directory = currentDirectory;
    directory = string.IsNullOrEmpty(folders) == false ? Path.Combine(directory, folders) : directory;
    directory = string.IsNullOrEmpty(file) == false ? Path.Combine(directory, file) : directory;

    return directory;
  }

  public static IConfigurationBuilder AddApplicationConsoleConfigurations(this IConfigurationBuilder configBuilder)
  {
    // Configure the appsettings.json
    var appSettingsPath = GetApplicationPath(file: "appsettings.json", folders: "Startup");
    configBuilder.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);

    return configBuilder;
  }
}