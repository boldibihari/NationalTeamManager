namespace NationalTeamManager.Domain.Entities
{
    public class Match
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Opponent { get; set; } = null!;
        public bool IsHome { get; set; }
        public int? HungaryScore { get; set; }
        public int? OpponentScore { get; set; }
        public string? Venue { get; set; }
        public int CompetitionId { get; set; }
        public Competition Competition { get; set; } = null!;
        public string? ExternalId { get; set; }
        public string? DataSource { get; set; }
    }
}
