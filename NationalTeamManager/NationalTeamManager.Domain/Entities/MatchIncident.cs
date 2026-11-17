using NationalTeamManager.Domain.Interfaces;

namespace NationalTeamManager.Domain.Entities
{
    public class MatchIncident : IEntity
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public string ExternalKey { get; set; } = null!;
        public string? ExternalId { get; set; }
        public string? DataSource { get; set; }
        public string? IncidentType { get; set; }
        public string? IncidentClass { get; set; }
        public int? Time { get; set; }
        public int? AddedTime { get; set; }
        public int? ReversedPeriodTime { get; set; }
        public bool? IsHome { get; set; }
        public string? PlayerExternalId { get; set; }
        public string? PlayerName { get; set; }
        public string? AssistPlayerExternalId { get; set; }
        public string? AssistPlayerName { get; set; }
        public string? PlayerInExternalId { get; set; }
        public string? PlayerInName { get; set; }
        public string? PlayerOutExternalId { get; set; }
        public string? PlayerOutName { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string? Reason { get; set; }
        public bool? Rescinded { get; set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
    }
}
