using NationalTeamManager.DataImporter.Providers.SofaScore.Models;

namespace NationalTeamManager.DataImporter.Providers.SofaScore
{
    public interface ISofaScoreClient
    {
        Task<SofaScoreSquadResponse> GetSquadAsync(
            int teamId,
            CancellationToken cancellationToken = default
        );
    }
}
