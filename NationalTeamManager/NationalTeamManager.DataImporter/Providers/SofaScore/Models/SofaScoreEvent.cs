using System.Text.Json.Serialization;

namespace NationalTeamManager.DataImporter.Providers.SofaScore.Models
{
    public class SofaScoreEvent
    {
        [JsonPropertyName("tournament")]
        public SofaScoreTournament? Tournament { get; set; }

        [JsonPropertyName("season")]
        public SofaScoreSeason? Season { get; set; }

        [JsonPropertyName("roundInfo")]
        public SofaScoreRoundInfo? RoundInfo { get; set; }

        [JsonPropertyName("status")]
        public SofaScoreEventStatus? Status { get; set; }

        [JsonPropertyName("homeTeam")]
        public SofaScoreMatchTeam HomeTeam { get; set; } = null!;

        [JsonPropertyName("awayTeam")]
        public SofaScoreMatchTeam AwayTeam { get; set; } = null!;

        [JsonPropertyName("homeScore")]
        public SofaScoreScore? HomeScore { get; set; }

        [JsonPropertyName("awayScore")]
        public SofaScoreScore? AwayScore { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("startTimestamp")]
        public long StartTimestamp { get; set; }
    }
}
