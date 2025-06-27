using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;
using System.Threading;
using System.Xml;

namespace Persistence.Repositories
{
    public class UserRepository : TRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<Result<User>> GetByEmailAsync(Result<Email> email, CancellationToken cancellationToken = default)
        {
            if (email.IsFailure) return Result.Failure<User>(email.Error);
            return await GetByPredicateAsync(u => u.Email == email.Value, cancellationToken);
        }

        public async Task<Result<User>> GetByUsernameAsync(Result<Username> username, CancellationToken cancellationToken = default)
        {
            if (username.IsFailure) return Result.Failure<User>(username.Error);
            return await GetByPredicateAsync(u => u.Username == username.Value, cancellationToken);
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

        protected async override Task<Result<User>> VerificationBeforeAddingAsync(Result<User> newUser, CancellationToken cancellationToken)
        {
            if (newUser.IsFailure) return newUser;

            Result<bool> unique;

            unique = await IsEmailUniqueAsync(newUser.Value.Email, cancellationToken);
            if (unique.IsFailure) return Result.Failure<User>(unique.Error);
            if (!unique.Value) return Result.Failure<User>(PersistenceErrors.User.EmailNotUnique);

            unique = await IsUsernameUniqueAsync(newUser.Value.Username, cancellationToken);
            if (unique.IsFailure) return Result.Failure<User>(unique.Error);
            if (!unique.Value) { return Result.Failure<User>(PersistenceErrors.User.UsernameNotUnique); }

            var user = await GetByIdAsync(newUser.Value.Id, cancellationToken);
            if (user.IsSuccess) return Result.Failure<User>(PersistenceErrors.User.AlreadyExists);

            return newUser;
        }
        protected override Error GetErrorIdEmpty()
        {
            return PersistenceErrors.User.NotFound;
        }

        protected override Error GetErrorNotFound()
        {
            return PersistenceErrors.User.NotFound;
        }

        protected async override Task<Result<User>> VerificationBeforeUpdateAsync(Result<User> entity, CancellationToken cancellationToken)
        {
            if (entity.IsFailure) return Result.Failure<User>(entity);
            return await GetByIdAsync(entity.Value.Id, cancellationToken);
        }
        
        protected async override Task<Result<User>> VerificationBeforeRemoveAsync(Result<User> entity, CancellationToken cancellationToken)
        {
            if (entity.IsFailure) return Result.Failure<User>(entity);
            return await GetByIdAsync(entity.Value.Id, cancellationToken);
        }
    }
}
