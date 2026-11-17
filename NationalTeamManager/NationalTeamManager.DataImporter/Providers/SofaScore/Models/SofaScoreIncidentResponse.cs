using System.Text.Json.Serialization;

namespace NationalTeamManager.DataImporter.Providers.SofaScore.Models
{
    public class SofaScoreIncidentResponse
    {
        [JsonPropertyName("incidents")]
        public List<SofaScoreIncident> Incidents { get; set; } = [];
    }

    public class SofaScoreIncident
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("incidentType")]
        public string? IncidentType { get; set; }

        [JsonPropertyName("incidentClass")]
        public string? IncidentClass { get; set; }

        [JsonPropertyName("time")]
        public int? Time { get; set; }

        [JsonPropertyName("addedTime")]
        public int? AddedTime { get; set; }

        [JsonPropertyName("length")]
        public int? Length { get; set; }

        [JsonPropertyName("timeSeconds")]
        public int? TimeSeconds { get; set; }

        [JsonPropertyName("periodTimeSeconds")]
        public int? PeriodTimeSeconds { get; set; }

        [JsonPropertyName("reversedPeriodTime")]
        public int? ReversedPeriodTime { get; set; }

        [JsonPropertyName("reversedPeriodTimeSeconds")]
        public int? ReversedPeriodTimeSeconds { get; set; }

        [JsonPropertyName("isHome")]
        public bool? IsHome { get; set; }

        [JsonPropertyName("player")]
        public SofaScoreIncidentPlayer? Player { get; set; }

        [JsonPropertyName("assist1")]
        public SofaScoreIncidentPlayer? Assist1 { get; set; }

        [JsonPropertyName("playerIn")]
        public SofaScoreIncidentPlayer? PlayerIn { get; set; }

        [JsonPropertyName("playerOut")]
        public SofaScoreIncidentPlayer? PlayerOut { get; set; }

        [JsonPropertyName("homeScore")]
        public int? HomeScore { get; set; }

        [JsonPropertyName("awayScore")]
        public int? AwayScore { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("rescinded")]
        public bool? Rescinded { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class SofaScoreIncidentPlayer
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
