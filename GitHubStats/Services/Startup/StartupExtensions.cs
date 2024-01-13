namespace Services.Startupp;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class StartupExtensions
{
  /// <summary>
  /// Dependency Injection logic for the application's services
  /// </summary>
  /// <param name="serviceCollection"></param>
  /// <returns>The configured application's service collection</returns>
  public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
  {
    // Configure the DI container for Services project
    serviceCollection.TryAddScoped<IGitHubRestService, GitHubRestService>();

    return serviceCollection;
  }
}