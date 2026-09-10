using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Persistence.Configurations
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);

            builder.Property(x => x.Country).HasMaxLength(100);

            builder.Property(x => x.ExternalId).HasMaxLength(100);

            builder.Property(x => x.DataSource).HasMaxLength(50);

            builder.HasIndex(x => new { x.ExternalId, x.DataSource }).IsUnique();
        }
    }
}
