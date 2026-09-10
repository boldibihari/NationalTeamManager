using Microsoft.EntityFrameworkCore;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Persistence
{
    public class NationalTeamManagerDbContext(
        DbContextOptions<NationalTeamManagerDbContext> options
    ) : DbContext(options)
    {
        public DbSet<Player> Players => Set<Player>();

        public DbSet<Club> Clubs => Set<Club>();

        public DbSet<Competition> Competitions => Set<Competition>();

        public DbSet<Match> Matches => Set<Match>();

        public DbSet<PlayerMarketValue> PlayerMarketValues => Set<PlayerMarketValue>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(NationalTeamManagerDbContext).Assembly
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
