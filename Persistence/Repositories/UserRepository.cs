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
            return await GetFromDBAsync(u => u.Email == email.Value, cancellationToken);
        }

        public Task<Result<User>> GetByUsernameAsync(Result<Username> username, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> IsEmailUniqueAsync(Result<Email> email, CancellationToken cancellationToken = default)
        {
            if (email.IsFailure) return Result.Failure<bool>(email.Error);
            return !await _dbContext.Set<User>().AnyAsync(u => u.Email == email.Value && u.IsSoftDelete == false, cancellationToken);
        }

        public async Task<Result<bool>> IsUsernameUniqueAsync(Result<Username> username, CancellationToken cancellationToken = default)
        {
            if (username.IsFailure) return Result.Failure<bool>(username.Error);
            return !await _dbContext.Set<User>().AnyAsync(u => u.Username == username.Value && u.IsSoftDelete == false, cancellationToken);
        }

        protected async override Task<Result<User>> VerificationBeforeAddingAsync(Result<User> entity, CancellationToken cancellationToken)
        {
            if (entity.IsFailure) return entity;

            Result<bool> unique;

            unique = await IsEmailUniqueAsync(entity.Value.Email, cancellationToken);
            if (unique.IsFailure) return Result.Failure<User>(unique.Error);
            if (!unique.Value) return Result.Failure<User>(PersistenceErrors.User.EmailNotUnique);

            unique = await IsUsernameUniqueAsync(entity.Value.Username, cancellationToken);
            if (unique.IsFailure) return Result.Failure<User>(unique.Error);
            if (!unique.Value) { return Result.Failure<User>(PersistenceErrors.User.UsernameNotUnique); }

            var user = await GetFromDBAsync(entity.Value.Id, cancellationToken);
            if (user.IsSuccess) return Result.Failure<User>(PersistenceErrors.User.AlreadyExists);

            return entity;
        }
        protected override Error GetErrorIdEmpty()
        {
            return PersistenceErrors.User.NotFound;
        }

        protected override Error GetErrorNotFound()
        {
            return PersistenceErrors.User.NotFound;
        }

        protected async override Task<Result<User>> VerificationBeforeRemoveAsync(Result<User> entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            if (entity.IsFailure) return entity;
        }

        protected async override Task<Result<User>> VerificationBeforeUpdateAsync(Result<User> entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            if (entity.IsFailure) return entity;
        }
    }
}
