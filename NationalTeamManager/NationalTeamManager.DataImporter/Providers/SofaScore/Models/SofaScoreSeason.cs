using System.Text.Json.Serialization;

namespace NationalTeamManager.DataImporter.Providers.SofaScore.Models
{
    public class SofaScoreSeason
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("year")]
        public string? Year { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
