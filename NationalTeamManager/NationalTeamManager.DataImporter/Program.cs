using Microsoft.Extensions.Hosting;
using NationalTeamManager.Infrastructure.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

using var host = builder.Build();

await host.RunAsync();
