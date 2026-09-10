using NationalTeamManager.Domain.Enums;

namespace NationalTeamManager.Domain.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public PlayerPosition Position { get; set; }
        public int? Height { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public PreferredFoot PreferredFoot { get; set; }
        public string? Nationality { get; set; }
        public string? ExternalId { get; set; }
        public string? DataSource { get; set; }
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public ICollection<PlayerMarketValue> MarketValues { get; set; } = [];
    }
}
