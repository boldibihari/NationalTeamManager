using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Persistence.Configurations
{
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

            builder.Property(x => x.Position).IsRequired();

            builder.Property(x => x.PreferredFoot).IsRequired();

            builder.Property(x => x.Nationality).HasMaxLength(100);

            builder.Property(x => x.ExternalId).HasMaxLength(100);

            builder.Property(x => x.DataSource).HasMaxLength(50);

            builder
                .HasOne(x => x.Team)
                .WithMany(x => x.Players)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => new { x.ExternalId, x.DataSource }).IsUnique();
        }
    }
}
