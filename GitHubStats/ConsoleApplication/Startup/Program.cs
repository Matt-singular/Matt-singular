using Microsoft.Extensions.Hosting;
using ConsoleApplication.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Configuration application console host
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => config.AddApplicationConsoleConfigurations())
    .Build();


// Keep console open by prompting input
Console.ReadLine();