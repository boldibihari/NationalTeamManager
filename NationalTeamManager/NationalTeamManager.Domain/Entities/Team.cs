using NationalTeamManager.Domain.Interfaces;

namespace NationalTeamManager.Domain.Entities
{
    public class Team : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
        public bool IsNationalTeam { get; set; }
        public string? ExternalId { get; set; }
        public string? DataSource { get; set; }
        public ICollection<Player> Players { get; set; } = [];
        public ICollection<Match> HomeMatches { get; set; } = [];
        public ICollection<Match> AwayMatches { get; set; } = [];
    }
}
