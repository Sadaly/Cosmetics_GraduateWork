using Domain.Abstractions;
using Domain.Shared;

namespace Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
	private readonly AppDbContext _dbContext;

	public UnitOfWork(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<Result<T>> SaveChangesAsync<T>(Result<T> result, CancellationToken cancellationToken = default)
	{
		if (result.IsFailure) return result;

		await _dbContext.SaveChangesAsync(cancellationToken);
		return result;
	}
}
