using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NationalTeamManager.DataImporter.Configuration;
using NationalTeamManager.DataImporter.Importers;
using NationalTeamManager.DataImporter.Importers;
using NationalTeamManager.DataImporter.Providers.SofaScore;
using NationalTeamManager.Infrastructure.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.Configure<RapidApiOptions>(
    builder.Configuration.GetSection(RapidApiOptions.SectionName)
);

builder.Services.AddHttpClient(
    "SofaScore",
    (serviceProvider, client) =>
    {
        var options = serviceProvider
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<RapidApiOptions>>()
            .Value;

        client.BaseAddress = new Uri(options.BaseUrl);
        client.DefaultRequestHeaders.Add("X-RapidAPI-Key", options.ApiKey);
        client.DefaultRequestHeaders.Add("X-RapidAPI-Host", options.Host);
    }
);

builder.Services.AddScoped<ISofaScoreClient, SofaScoreClient>();
builder.Services.AddScoped<PlayerImporter>();
builder.Services.AddScoped<MatchImporter>();

builder.Services.AddInfrastructure(builder.Configuration);

using var host = builder.Build();

await using var scope = host.Services.CreateAsyncScope();

var importer = scope.ServiceProvider.GetRequiredService<PlayerImporter>();

await importer.ImportAsync(4709);

var matchImporter = scope.ServiceProvider.GetRequiredService<MatchImporter>();

await matchImporter.ImportAsync(4709);

Console.WriteLine("Import sikeresen lefutott.");
