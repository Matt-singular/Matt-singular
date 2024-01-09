using Microsoft.Extensions.Hosting;
using ConsoleApplication.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Configuration application console host
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => config.AddApplicationConsoleConfigurations())
    .Build();

// Testing logic
var configuration = host.Services.GetRequiredService<IConfiguration>();
var testSecret = configuration["SecretTest"];
Console.WriteLine("Testing appsettings, GitHub secrets and user secrets setup using placeholder values that can be exposed\n");
Console.WriteLine($"Secret value = {testSecret}");

// Keep console open by prompting input
Console.ReadLine();