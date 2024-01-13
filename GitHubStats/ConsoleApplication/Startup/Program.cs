using Microsoft.Extensions.Hosting;
using ConsoleApplication.Startup;
using Services.Startupp;
using Microsoft.Extensions.DependencyInjection;

// Configuration application console host
var host = Host.CreateDefaultBuilder(args)
  .ConfigureAppConfiguration(config => config.AddApplicationConsoleConfigurations())
  .ConfigureServices(services => services.AddApplicationServices())
  .Build();

// Temporary for debugging purposese
var gitHubService = host.Services.GetRequiredService<Services.IGitHubRestService>();
var gitHubStatisticDetails = await gitHubService.GetGitHubUserStatistics().ConfigureAwait(false);
//await gitHubService.GetGitHubDiffForRange().ConfigureAwait(false);

// Keep console open by prompting input
Console.Write("Application built successfully... ");
Console.ReadLine();