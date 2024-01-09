namespace ConsoleApplication.Tests.Helpers;

using ConsoleApplication.Helpers;

public class FileHelperTests
{
  [Theory]
  [InlineData(null)]
  [InlineData("fileName")]
  [InlineData("fileName", "folder1")]
  [InlineData("fileName", "folder1", "folder2")]
  public void TestGetApplicationPath(string? file, params string[] folders)
  {
    // Act
    var path = FileHelpers.GetApplicationPath(file, folders ?? []);

    // Assert
    path.Should().NotBeNull();
    if (file != null) path.Should().Contain(file);
    if (folders?.Any() ?? false) path.Should().ContainAll(folders);
  }
}