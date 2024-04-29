namespace TicketingSystem.BusinessLogic.Services
{
    public interface IService<TEntityDto>
    {
        Task<int> CreateAsync(TEntityDto entity, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<TEntityDto>> GetAllAsync(int? pageNumber = null, int? pageSize = null,
            CancellationToken cancellationToken = default);

        Task<TEntityDto> GetById(int entityId, CancellationToken cancellationToken = default);

        Task UpdateAsync(TEntityDto entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(int entityId, CancellationToken cancellationToken = default);
    }
}
