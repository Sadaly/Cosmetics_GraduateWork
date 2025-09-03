using Domain.Abstractions;
using Domain.Entity;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Repositories
{
	public interface IUserRepository : IRepository<User>
	{
		Task<Result<bool>> IsEmailUniqueAsync(Result<Email> email, CancellationToken cancellationToken = default);
		Task<Result<bool>> IsUsernameUniqueAsync(Result<Username> username, CancellationToken cancellationToken = default);
		Task<Result<User>> GetByUsernameAsync(Result<Username> username, CancellationToken cancellationToken = default);
		Task<Result<User>> GetByEmailAsync(Result<Email> email, CancellationToken cancellationToken = default);
	}
}
