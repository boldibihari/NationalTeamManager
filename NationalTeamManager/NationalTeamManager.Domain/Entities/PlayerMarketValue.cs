using NationalTeamManager.Domain.Interfaces;

namespace NationalTeamManager.Domain.Entities
{
    public class PlayerMarketValue : IEntity
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;
        public decimal Value { get; set; }
        public string Currency { get; set; } = "EUR";
        public DateTime RecordedAt { get; set; }
    }
}
