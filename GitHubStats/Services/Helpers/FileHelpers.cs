namespace Services.Helpers;

public static class FileHelpers
{
  /// <summary>
  /// A file helper to get the application path including the optional file and folders provided
  /// (this function will provide the correct application path regardless of the build configuration used and whether it was run in GitHub action or locally)
  /// </summary>
  /// <param name="file">Optional file name parameter</param>
  /// <param name="folders">Optional folder names</param>
  /// <returns>The application's path</returns>
  public static string GetApplicationPath(string? file = null, params string[] folders)
  {
    // Base directory path
    var applicationPath = Directory.GetCurrentDirectory();

    // Include project folder if running via GitHub actions (not locally)
#if ACTION
    // This logic is required when run via a GitHub action as the current directory will be "GitHubStats" not "ConsoleApplication"
    applicationPath = string.Join(Constants.PathSeparator, Constants.ConsoleApplicationProject);
#endif

    // Include path/folders if provided
    applicationPath = folders.Length > 0 ? string.Join(Constants.PathSeparator, new string[] { applicationPath }.Concat(folders)) : applicationPath;
    applicationPath = string.IsNullOrEmpty(file) == false ? string.Join(Constants.PathSeparator, applicationPath, file) : string.Empty;

    return applicationPath;
  }


  /// <summary>
  /// Localised FileHelper constants
  /// </summary>
  public static class Constants
  {
    public const char PathSeparator = '/';
    public const string ConsoleApplicationProject = "ConsoleApplication";
  }
}