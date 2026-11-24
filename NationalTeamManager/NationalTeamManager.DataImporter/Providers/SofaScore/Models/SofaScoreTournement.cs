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

        [JsonPropertyName("groupName")]
        public string? GroupName { get; set; }

        [JsonPropertyName("isGroup")]
        public bool? IsGroup { get; set; }
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
