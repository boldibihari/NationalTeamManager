using NationalTeamManager.DataImporter.Providers.SofaScore.Models;

namespace NationalTeamManager.DataImporter.Providers.SofaScore
{
    public interface ISofaScoreClient
    {
        Task<SofaScoreSquadResponse> GetSquadAsync(
            int teamId,
            CancellationToken cancellationToken = default
        );

        Task<SofaScoreMatchResponse> GetLastMatchesAsync(
            int teamId,
            CancellationToken cancellationToken = default
        );

        Task<SofaScoreMatchResponse> GetNextMatchesAsync(
            int teamId,
            CancellationToken cancellationToken = default
        );
    }
}
