using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class UserRepository : TRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<Result<User>> GetByEmailAsync(Result<Email> email, CancellationToken cancellationToken = default)
        {
            if (email.IsFailure) return Result.Failure<User>(email.Error);
            return await GetFromDBAsync(u => u.Email == email.Value, PersistenceErrors.User.NotFound, cancellationToken);
        }

        public Task<Result<User>> GetByUsernameAsync(Result<Username> username, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> IsEmailUniqueAsync(Result<Email> email, CancellationToken cancellationToken = default)
        {
            if (email.IsFailure) return Result.Failure<bool>(email.Error);
            return !await _dbContext.Set<User>().AnyAsync(u => u.Email == email.Value, cancellationToken);
        }

        public async Task<Result<bool>> IsUsernameUniqueAsync(Result<Username> username, CancellationToken cancellationToken = default)
        {
            if (username.IsFailure) return Result.Failure<bool>(username.Error);
            return !await _dbContext.Set<User>().AnyAsync(u => u.Username == username.Value, cancellationToken);
        }

        protected async override Task<Result> VerificationBeforeAddingAsync(Result<User> entity, CancellationToken cancellationToken)
        {
            Result<bool> unique;

            unique = await IsEmailUniqueAsync(entity.Value.Email, cancellationToken);
            if (unique.IsFailure) { return unique; }
            if (!unique.Value) { return Result.Failure(PersistenceErrors.User.EmailNotUnique); }

            unique = await IsUsernameUniqueAsync(entity.Value.Username, cancellationToken);
            if (unique.IsFailure) { return unique; }
            if (!unique.Value) { return Result.Failure(PersistenceErrors.User.UsernameNotUnique); }

            var user = await GetFromDBAsync(entity.Value.Id, cancellationToken);
            if (user.IsSuccess) { return Result.Failure(PersistenceErrors.User.AlreadyExists); }
            return Result.Success();
        }
        protected override Error GetErrorIdEmpty()
        {
            return PersistenceErrors.User.NotFound;
        }

        protected override Error GetErrorNotFound()
        {
            return PersistenceErrors.User.NotFound;
        }

        protected override Task<Result> VerificationBeforeRemoveAsync(Result<User> entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<Result> VerificationBeforeUpdateAsync(Result<User> entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
