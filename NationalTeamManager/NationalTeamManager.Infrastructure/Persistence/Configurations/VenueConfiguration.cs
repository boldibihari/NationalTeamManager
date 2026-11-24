using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Persistence.Configurations
{
    public class VenueConfiguration : IEntityTypeConfiguration<Venue>
    {
        public void Configure(EntityTypeBuilder<Venue> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.City).HasMaxLength(150);
            builder.Property(x => x.Country).HasMaxLength(100);
            builder.Property(x => x.Surface).HasMaxLength(100);
            builder.Property(x => x.Latitude).HasPrecision(9, 6);
            builder.Property(x => x.Longitude).HasPrecision(9, 6);
            builder.Property(x => x.ExternalId).HasMaxLength(100);
            builder.Property(x => x.DataSource).HasMaxLength(50);

            builder.HasIndex(x => new { x.ExternalId, x.DataSource }).IsUnique();
        }
    }
}
