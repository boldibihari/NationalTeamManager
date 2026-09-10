namespace NationalTeamManager.Application.Interfaces
{
    public interface ICrudService<TDto, TCreateDto, TUpdateDto>
    {
        Task<List<TDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<TDto> AddAsync(TCreateDto dto, CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            int id,
            TUpdateDto dto,
            CancellationToken cancellationToken = default
        );

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
