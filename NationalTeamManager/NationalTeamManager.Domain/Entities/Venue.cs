using NationalTeamManager.Domain.Interfaces;

namespace NationalTeamManager.Domain.Entities
{
    public class Venue : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? City { get; set; }
        public string? Country { get; set; }
        public int? Capacity { get; set; }
        public string? Surface { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? ExternalId { get; set; }
        public string? DataSource { get; set; }
        public ICollection<Match> Matches { get; set; } = [];
    }
}
