using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Persistence.Configurations
{
    public class MatchConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Date).IsRequired();

            builder.Property(x => x.ExternalId).HasMaxLength(100);

            builder.Property(x => x.DataSource).HasMaxLength(50);

            builder.Property(x => x.CustomId).HasMaxLength(100);

            builder.Property(x => x.Slug).HasMaxLength(200);

            builder.Property(x => x.DetailsImportedAt);

            builder.Property(x => x.StatusType).HasMaxLength(50);

            builder.Property(x => x.StatusDescription).HasMaxLength(100);

            builder.Property(x => x.TournamentGroupName).HasMaxLength(150);

            builder
                .HasOne(x => x.HomeTeam)
                .WithMany(x => x.HomeMatches)
                .HasForeignKey(x => x.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.AwayTeam)
                .WithMany(x => x.AwayMatches)
                .HasForeignKey(x => x.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.CompetitionEdition)
                .WithMany(x => x.Matches)
                .HasForeignKey(x => x.CompetitionEditionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.CompetitionStage)
                .WithMany(x => x.Matches)
                .HasForeignKey(x => x.CompetitionStageId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasOne(x => x.Venue)
                .WithMany(x => x.Matches)
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasOne(x => x.Referee)
                .WithMany(x => x.Matches)
                .HasForeignKey(x => x.RefereeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => new { x.ExternalId, x.DataSource }).IsUnique();
        }
    }
}
