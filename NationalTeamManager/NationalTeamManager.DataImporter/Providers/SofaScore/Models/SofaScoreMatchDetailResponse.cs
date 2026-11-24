using System.Text.Json.Serialization;

namespace NationalTeamManager.DataImporter.Providers.SofaScore.Models
{
    public class SofaScoreMatchDetailResponse
    {
        [JsonPropertyName("event")]
        public SofaScoreMatchDetail? Event { get; set; }
    }

    public class SofaScoreMatchDetail : SofaScorePeriodDefaults
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
        public SofaScoreMatchTeam? HomeTeam { get; set; }

        [JsonPropertyName("awayTeam")]
        public SofaScoreMatchTeam? AwayTeam { get; set; }

        [JsonPropertyName("homeScore")]
        public SofaScoreScore? HomeScore { get; set; }

        [JsonPropertyName("awayScore")]
        public SofaScoreScore? AwayScore { get; set; }

        [JsonPropertyName("time")]
        public SofaScoreMatchTime? Time { get; set; }

        [JsonPropertyName("winnerCode")]
        public int? WinnerCode { get; set; }

        [JsonPropertyName("venue")]
        public SofaScoreVenue? Venue { get; set; }

        [JsonPropertyName("referee")]
        public SofaScoreReferee? Referee { get; set; }

        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("customId")]
        public string? CustomId { get; set; }

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("startTimestamp")]
        public long? StartTimestamp { get; set; }
    }

    public abstract class SofaScorePeriodDefaults
    {
        [JsonPropertyName("defaultPeriodLength")]
        public int? DefaultPeriodLength { get; set; }

        [JsonPropertyName("defaultOvertimeLength")]
        public int? DefaultOvertimeLength { get; set; }

        [JsonPropertyName("defaultPeriodCount")]
        public int? DefaultPeriodCount { get; set; }
    }

    public class SofaScoreMatchTime : SofaScorePeriodDefaults
    {
        [JsonPropertyName("periodLength")]
        public int? PeriodLength { get; set; }

        [JsonPropertyName("overtimeLength")]
        public int? OvertimeLength { get; set; }

        [JsonPropertyName("totalPeriodCount")]
        public int? TotalPeriodCount { get; set; }

        [JsonPropertyName("initial")]
        public int? Initial { get; set; }

        [JsonPropertyName("max")]
        public int? Max { get; set; }

        [JsonPropertyName("extra")]
        public int? Extra { get; set; }

        [JsonPropertyName("currentPeriodStartTimestamp")]
        public long? CurrentPeriodStartTimestamp { get; set; }

        [JsonPropertyName("injuryTime1")]
        public int? InjuryTime1 { get; set; }

        [JsonPropertyName("injuryTime2")]
        public int? InjuryTime2 { get; set; }

        [JsonPropertyName("injuryTime3")]
        public int? InjuryTime3 { get; set; }

        [JsonPropertyName("injuryTime4")]
        public int? InjuryTime4 { get; set; }
    }

    public class SofaScoreVenue
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("city")]
        public SofaScoreVenueCity? City { get; set; }

        [JsonPropertyName("country")]
        public SofaScoreCountry? Country { get; set; }

        [JsonPropertyName("stadium")]
        public SofaScoreStadium? Stadium { get; set; }

        [JsonPropertyName("surfaceType")]
        public string? SurfaceType { get; set; }

        [JsonPropertyName("capacity")]
        public int? Capacity { get; set; }

        [JsonPropertyName("latitude")]
        public decimal? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public decimal? Longitude { get; set; }

        [JsonPropertyName("venueCoordinates")]
        public SofaScoreCoordinates? VenueCoordinates { get; set; }
    }

    public class SofaScoreVenueCity
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class SofaScoreStadium
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("capacity")]
        public int? Capacity { get; set; }

        [JsonPropertyName("surface")]
        public string? Surface { get; set; }

        [JsonPropertyName("fieldType")]
        public string? FieldType { get; set; }

        [JsonPropertyName("coordinates")]
        public SofaScoreCoordinates? Coordinates { get; set; }
    }

    public class SofaScoreCoordinates
    {
        [JsonPropertyName("latitude")]
        public decimal? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public decimal? Longitude { get; set; }
    }

    public class SofaScoreReferee
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("country")]
        public SofaScoreCountry? Country { get; set; }
    }
}
