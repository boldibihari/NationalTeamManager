namespace NationalTeamManager.Domain.Entities
{
    public class Club
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
        public string? ExternalId { get; set; }
        public string? DataSource { get; set; }
        public ICollection<Player> Players { get; set; } = [];
    }
}
