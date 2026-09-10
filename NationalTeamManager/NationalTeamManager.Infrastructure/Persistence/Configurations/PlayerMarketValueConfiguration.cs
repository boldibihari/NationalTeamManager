using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Persistence.Configurations
{
    public class PlayerMarketValueConfiguration : IEntityTypeConfiguration<PlayerMarketValue>
    {
        public void Configure(EntityTypeBuilder<PlayerMarketValue> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value).HasPrecision(18, 2);

            builder.Property(x => x.Currency).IsRequired().HasMaxLength(3);

            builder.Property(x => x.RecordedAt).IsRequired();

            builder
                .HasOne(x => x.Player)
                .WithMany(x => x.MarketValues)
                .HasForeignKey(x => x.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.PlayerId, x.RecordedAt }).IsUnique();
        }
    }
}
