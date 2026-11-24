using NationalTeamManager.Domain.Interfaces;

namespace NationalTeamManager.Domain.Entities
{
    public class Match : IEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;
        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public int? VenueId { get; set; }
        public Venue? Venue { get; set; }
        public int? RefereeId { get; set; }
        public Referee? Referee { get; set; }
        public int CompetitionEditionId { get; set; }
        public CompetitionEdition CompetitionEdition { get; set; } = null!;
        public int? CompetitionStageId { get; set; }
        public CompetitionStage? CompetitionStage { get; set; }
        public string? ExternalId { get; set; }
        public string? DataSource { get; set; }
        public string? CustomId { get; set; }
        public string? Slug { get; set; }
        public DateTime? DetailsImportedAt { get; set; }
        public string? StatusType { get; set; }
        public int? StatusCode { get; set; }
        public string? StatusDescription { get; set; }
        public int? WinnerCode { get; set; }
        public string? TournamentGroupName { get; set; }
        public int? HomeScoreDisplay { get; set; }
        public int? AwayScoreDisplay { get; set; }
        public int? HomeScorePeriod1 { get; set; }
        public int? AwayScorePeriod1 { get; set; }
        public int? HomeScorePeriod2 { get; set; }
        public int? AwayScorePeriod2 { get; set; }
        public int? HomeScoreNormalTime { get; set; }
        public int? AwayScoreNormalTime { get; set; }
        public int? HomeScoreOvertime { get; set; }
        public int? AwayScoreOvertime { get; set; }
        public int? HomeScorePenalties { get; set; }
        public int? AwayScorePenalties { get; set; }
        public int? TimePeriodLength { get; set; }
        public int? TimeOvertimeLength { get; set; }
        public int? TimeTotalPeriodCount { get; set; }
        public long? TimeCurrentPeriodStartTimestamp { get; set; }
        public int? TimeInjuryTime1 { get; set; }
        public int? TimeInjuryTime2 { get; set; }
        public int? TimeInjuryTime3 { get; set; }
        public int? TimeInjuryTime4 { get; set; }
        public ICollection<MatchIncident> Incidents { get; set; } = [];
    }
}
