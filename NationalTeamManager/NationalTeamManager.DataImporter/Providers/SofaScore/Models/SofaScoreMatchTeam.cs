using System.Text.Json.Serialization;

namespace NationalTeamManager.DataImporter.Providers.SofaScore.Models
{
    public class SofaScoreMatchTeam
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("shortName")]
        public string? ShortName { get; set; }

        [JsonPropertyName("nameCode")]
        public string? NameCode { get; set; }

        [JsonPropertyName("country")]
        public SofaScoreCountry? Country { get; set; }

        [JsonPropertyName("national")]
        public bool National { get; set; }
    }
}
