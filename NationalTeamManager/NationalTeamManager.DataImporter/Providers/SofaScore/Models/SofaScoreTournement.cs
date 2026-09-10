using System.Text.Json.Serialization;

namespace NationalTeamManager.DataImporter.Providers.SofaScore.Models
{
    public class SofaScoreTournament
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("uniqueTournament")]
        public SofaScoreUniqueTournament? UniqueTournament { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class SofaScoreUniqueTournament
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
