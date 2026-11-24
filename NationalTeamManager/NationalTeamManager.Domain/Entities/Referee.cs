using NationalTeamManager.Domain.Interfaces;

namespace NationalTeamManager.Domain.Entities
{
    public class Referee : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
        public string? ExternalId { get; set; }
        public string? DataSource { get; set; }
        public ICollection<Match> Matches { get; set; } = [];
    }
}
