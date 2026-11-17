using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Persistence.Configurations
{
    public class MatchIncidentConfiguration : IEntityTypeConfiguration<MatchIncident>
    {
        public void Configure(EntityTypeBuilder<MatchIncident> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ExternalKey).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ExternalId).HasMaxLength(100);
            builder.Property(x => x.DataSource).HasMaxLength(50);
            builder.Property(x => x.IncidentType).HasMaxLength(50);
            builder.Property(x => x.IncidentClass).HasMaxLength(100);
            builder.Property(x => x.PlayerExternalId).HasMaxLength(100);
            builder.Property(x => x.PlayerName).HasMaxLength(150);
            builder.Property(x => x.AssistPlayerExternalId).HasMaxLength(100);
            builder.Property(x => x.AssistPlayerName).HasMaxLength(150);
            builder.Property(x => x.PlayerInExternalId).HasMaxLength(100);
            builder.Property(x => x.PlayerInName).HasMaxLength(150);
            builder.Property(x => x.PlayerOutExternalId).HasMaxLength(100);
            builder.Property(x => x.PlayerOutName).HasMaxLength(150);
            builder.Property(x => x.Reason).HasMaxLength(200);
            builder.Property(x => x.Text).HasMaxLength(500);
            builder.Property(x => x.Description).HasMaxLength(1000);

            builder
                .HasOne(x => x.Match)
                .WithMany(x => x.Incidents)
                .HasForeignKey(x => x.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.DataSource, x.ExternalKey }).IsUnique();
        }
    }
}
