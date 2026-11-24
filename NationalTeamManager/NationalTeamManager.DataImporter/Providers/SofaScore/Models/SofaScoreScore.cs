using System.Text.Json.Serialization;

namespace NationalTeamManager.DataImporter.Providers.SofaScore.Models
{
    public class SofaScoreScore
    {
        [JsonPropertyName("current")]
        public int? Current { get; set; }

        [JsonPropertyName("display")]
        public int? Display { get; set; }

        [JsonPropertyName("normaltime")]
        public int? NormalTime { get; set; }

        [JsonPropertyName("period1")]
        public int? Period1 { get; set; }

        [JsonPropertyName("period2")]
        public int? Period2 { get; set; }

        [JsonPropertyName("extra1")]
        public int? Extra1 { get; set; }

        [JsonPropertyName("extra2")]
        public int? Extra2 { get; set; }

        [JsonPropertyName("overtime")]
        public int? Overtime { get; set; }

        [JsonPropertyName("penalties")]
        public int? Penalties { get; set; }
    }
}
