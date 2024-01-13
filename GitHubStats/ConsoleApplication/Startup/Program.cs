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
var testService = host.Services.GetRequiredService<Services.IGitHubRestService>();
await testService.GetGitHubUserStatistics().ConfigureAwait(false);
//await testService.GetGitHubDiffForRange().ConfigureAwait(false);
//var testService = host.Services.GetRequiredService<Services.IGitHubGraphQlService>();
//await testService.GetCommitStatisticsForUser().ConfigureAwait(false);

// Keep console open by prompting input
Console.Write("Application built successfully... ");
Console.ReadLine();