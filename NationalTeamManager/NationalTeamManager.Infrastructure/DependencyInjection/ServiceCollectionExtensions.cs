using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NationalTeamManager.Application.Common.Interfaces;
using NationalTeamManager.Application.Players.Interfaces;
using NationalTeamManager.Application.Team.Interfaces;
using NationalTeamManager.Infrastructure.Mapping;
using NationalTeamManager.Infrastructure.Persistence;
using NationalTeamManager.Infrastructure.Persistence.Repositories;
using NationalTeamManager.Infrastructure.Services.Entities;

namespace NationalTeamManager.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<NationalTeamManagerDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));

            MapsterConfiguration.Register();

            services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            services.AddScoped<IMapper, ServiceMapper>();

            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<ITeamService, TeamService>();

            return services;
        }
    }
}
