using System.Text.Json.Serialization;

namespace NationalTeamManager.DataImporter.Providers.SofaScore.Models
{
    public class SofaScoreSquadResponse
    {
        [JsonPropertyName("players")]
        public List<SofaScoreSquadPlayer> Players { get; set; } = [];
    }
}
