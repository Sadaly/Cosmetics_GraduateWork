using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using Persistence.Abstractions;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class UserRepository : TRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext) { }

        public Task<Result<User>> GetByEmailAsync(Result<Email> email, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<User>> GetByUsernameAsync(Result<Username> username, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> IsEmailUniqueAsync(Result<Email> email, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> IsUsernameUniqueAsync(Result<Username> username, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override Task<Result> VerificationBeforeAddingAsync(Result<User> entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
