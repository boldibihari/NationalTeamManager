using System.Text.Json.Serialization;

namespace NationalTeamManager.DataImporter.Providers.SofaScore.Models
{
    public class SofaScoreMatchResponse
    {
        [JsonPropertyName("events")]
        public List<SofaScoreEvent> Events { get; set; } = [];

        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage { get; set; }
    }
}
