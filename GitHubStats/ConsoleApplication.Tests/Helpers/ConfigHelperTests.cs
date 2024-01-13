namespace Services.Tests.Helpers;

using Microsoft.Extensions.Configuration;
using Services.Helpers;

public class ConfigHelperTests
{
  [Fact]
  public void TestGetConfigValue_ShouldMatchExpected()
  {
    // Arrage
    var mockedConfigValue = "TestValue";
    var configuration = Substitute.For<IConfiguration>();
    configuration[ConfigHelpers.Constants.AccessTokenKey].Returns(mockedConfigValue);

    // Act
    var configValue = configuration.GetConfigValue(ConfigHelpers.Constants.AccessTokenKey);

    // Assert
    configValue.Should().Be(mockedConfigValue);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("nonExistingKeys")]
  public void TestGetConfigValue(string? key)
  {
    // Arrage
    var configuration = Substitute.For<IConfiguration>();

    // Act
    var configValue = configuration.GetConfigValue(null);

    // Assert
    configValue.Should().BeNull();
  }
}