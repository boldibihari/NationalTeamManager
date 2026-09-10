using System.Text.Json.Serialization;

namespace NationalTeamManager.DataImporter.Providers.SofaScore.Models
{
    public class SofaScoreRoundInfo
    {
        [JsonPropertyName("round")]
        public int? Round { get; set; }
    }
}
