using MapsterMapper;
using NationalTeamManager.Application.Common.Interfaces;
using NationalTeamManager.Domain.Interfaces;

namespace NationalTeamManager.Infrastructure.Services.Common
{
    public abstract class CrudService<TEntity, TDto, TCreateDto, TUpdateDto>(
        IMapper mapper,
        IEntityRepository<TEntity> repository
    ) : ICrudService<TDto, TCreateDto, TUpdateDto>
        where TEntity : class, IEntity
        where TDto : class
    {
        protected readonly IMapper Mapper = mapper;
        protected readonly IEntityRepository<TEntity> Repository = repository;

        public virtual async Task<List<TDto>> GetAllAsync(
            CancellationToken cancellationToken = default
        )
        {
            var entities = await Repository.GetAllAsync(cancellationToken);

            return Mapper.Map<List<TDto>>(entities);
        }

        public virtual async Task<TDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            var entity = await Repository.GetByIdAsync(id, cancellationToken);

            return entity is null ? null : Mapper.Map<TDto>(entity);
        }

        public virtual async Task<TDto> AddAsync(
            TCreateDto dto,
            CancellationToken cancellationToken = default
        )
        {
            var entity = Mapper.Map<TEntity>(dto);

            await Repository.CreateAsync(entity, cancellationToken);

            return Mapper.Map<TDto>(entity);
        }

        public virtual async Task<bool> UpdateAsync(
            int id,
            TUpdateDto dto,
            CancellationToken cancellationToken = default
        )
        {
            var entity = await Repository.GetByIdAsync(id, cancellationToken);

            if (entity is null)
                return false;

            Mapper.Map(dto, entity);

            await Repository.UpdateAsync(entity, cancellationToken);

            return true;
        }

        public virtual async Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            var entity = await Repository.GetByIdAsync(id, cancellationToken);

            if (entity is null)
                return false;

            await Repository.DeleteAsync(id, cancellationToken);

            return true;
        }
    }
}
