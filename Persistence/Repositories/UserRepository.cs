using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Persistence.Abstractions;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class UserRepository : TRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext) { }
        protected override Task<Result> VerificationBeforeAddingAsync(User entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<Result> VerificationBeforeRemoveAsync(User entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<Result> VerificationBeforeUpdateAsync(User entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
