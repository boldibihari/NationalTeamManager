using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Persistence.Configurations
{
    public class CompetitionStageConfiguration : IEntityTypeConfiguration<CompetitionStage>
    {
        public void Configure(EntityTypeBuilder<CompetitionStage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);

            builder.Property(x => x.ExternalId).HasMaxLength(100);

            builder.Property(x => x.DataSource).HasMaxLength(50);

            builder
                .HasOne(x => x.CompetitionEdition)
                .WithMany(x => x.Stages)
                .HasForeignKey(x => x.CompetitionEditionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ExternalId, x.DataSource }).IsUnique();
        }
    }
}
