using Domain.Shared;

namespace Domain.Abstractions
{
    public interface IUnitOfWork
    {
        Task<Result<T>> SaveChangesAsync<T>(Result<T> result, CancellationToken cancellationToken = default);
    }
}
