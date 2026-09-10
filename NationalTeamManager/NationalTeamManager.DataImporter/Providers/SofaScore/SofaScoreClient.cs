using System.Net.Http.Json;
using NationalTeamManager.DataImporter.Providers.SofaScore.Models;

namespace NationalTeamManager.DataImporter.Providers.SofaScore
{
    public class SofaScoreClient(IHttpClientFactory httpClientFactory) : ISofaScoreClient
    {
        public async Task<SofaScoreSquadResponse> GetSquadAsync(
            int teamId,
            CancellationToken cancellationToken = default
        )
        {
            var client = httpClientFactory.CreateClient("SofaScore");

            var response = await client.GetAsync(
                $"teams/get-squad?teamId={teamId}",
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            var squad = await response.Content.ReadFromJsonAsync<SofaScoreSquadResponse>(
                cancellationToken
            );

            return squad
                ?? throw new InvalidOperationException("A SofaScore API üres választ adott.");
        }

        public async Task<SofaScoreMatchResponse> GetLastMatchesAsync(
            int teamId,
            CancellationToken cancellationToken = default
        )
        {
            var client = httpClientFactory.CreateClient("SofaScore");

            var response = await client.GetAsync(
                $"teams/get-last-matches?teamId={teamId}",
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            var matches = await response.Content.ReadFromJsonAsync<SofaScoreMatchResponse>(
                cancellationToken
            );

            return matches
                ?? throw new InvalidOperationException("A SofaScore API üres választ adott.");
        }

        public async Task<SofaScoreMatchResponse> GetNextMatchesAsync(
            int teamId,
            CancellationToken cancellationToken = default
        )
        {
            var client = httpClientFactory.CreateClient("SofaScore");

            var response = await client.GetAsync(
                $"teams/get-next-matches?teamId={teamId}",
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            var matches = await response.Content.ReadFromJsonAsync<SofaScoreMatchResponse>(
                cancellationToken
            );

            return matches
                ?? throw new InvalidOperationException("A SofaScore API üres választ adott.");
        }
    }
}
