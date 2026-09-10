using System.Text.Json.Serialization;

namespace NationalTeamManager.DataImporter.Providers.SofaScore.Models
{
    public class SofaScoreSquadPlayer
    {
        [JsonPropertyName("player")]
        public SofaScorePlayer Player { get; set; } = null!;
    }

    public class SofaScorePlayer
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("position")]
        public string? Position { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public DateTimeOffset? DateOfBirth { get; set; }

        [JsonPropertyName("preferredFoot")]
        public string? PreferredFoot { get; set; }

        [JsonPropertyName("sofascoreId")]
        public string? SofaScoreId { get; set; }

        [JsonPropertyName("country")]
        public SofaScoreCountry? Country { get; set; }

        [JsonPropertyName("team")]
        public SofaScoreTeam? Team { get; set; }

        [JsonPropertyName("proposedMarketValue")]
        public decimal? ProposedMarketValue { get; set; }

        [JsonPropertyName("proposedMarketValueRaw")]
        public SofaScoreMarketValue? ProposedMarketValueRaw { get; set; }
    }

    public class SofaScoreCountry
    {
        [JsonPropertyName("alpha2")]
        public string? Alpha2 { get; set; }

        [JsonPropertyName("alpha3")]
        public string? Alpha3 { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class SofaScoreMarketValue
    {
        [JsonPropertyName("value")]
        public decimal? Value { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
    }
}
