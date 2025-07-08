using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class UserRepository(AppDbContext dbContext) 
        : TRepository<User>(dbContext), IUserRepository
    {
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
            if (user.IsSuccess) return Result.Failure<User>(PersistenceErrors.Entity<User>.AlreadyExists);

            return newUser;
        }

        protected async override Task<Result<User>> VerificationBeforeUpdateAsync(Result<User> newEntity, CancellationToken cancellationToken)
        {
            if (newEntity.IsFailure) return newEntity;

            var oldEntity = await GetByIdAsync(newEntity.Value.Id, cancellationToken, Domain.Abstractions.FetchMode.NoTracking);
            if (oldEntity.IsFailure) return oldEntity;

            Result<bool> unique = Result.Success<bool>(false);

            if (newEntity.Value.Email.Value != oldEntity.Value.Email.Value)
            {
                unique = await IsEmailUniqueAsync(newEntity.Value.Email, cancellationToken);
                if (unique.IsFailure) return Result.Failure<User>(unique.Error);
                if (!unique.Value) return Result.Failure<User>(PersistenceErrors.User.EmailNotUnique);
            }

            if (newEntity.Value.Username.Value != oldEntity.Value.Username.Value)
            {
                unique = await IsUsernameUniqueAsync(newEntity.Value.Username, cancellationToken);
                if (unique.IsFailure) return Result.Failure<User>(unique.Error);
                if (!unique.Value) return Result.Failure<User>(PersistenceErrors.User.UsernameNotUnique);
            }
            if (newEntity.Value.PasswordHashed.Value != oldEntity.Value.PasswordHashed.Value) unique = Result.Success<bool>(true); 
            if (!unique.Value) return Result.Failure<User>(PersistenceErrors.User.UpdateChangeNothing);
            return newEntity;
        }
    }
}
