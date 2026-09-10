using System.Text.Json.Serialization;

namespace NationalTeamManager.DataImporter.Providers.SofaScore.Models
{
    public class SofaScoreEventStatus
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
