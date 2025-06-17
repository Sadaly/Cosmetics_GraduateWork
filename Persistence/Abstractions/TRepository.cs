using Domain.Abstractions;
using Domain.Common;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Abstractions
{
    public abstract class TRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public TRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        protected abstract Task<Result> VerificationBeforeAddingAsync(Result<T> entity, CancellationToken cancellationToken);
        protected abstract Task<Result> VerificationBeforeUpdateAsync(Result<T> entity, CancellationToken cancellationToken);
        protected abstract Task<Result> VerificationBeforeRemoveAsync(Result<T> entity, CancellationToken cancellationToken);

        public Task<Result> AddAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveAsync(Guid entityId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<T>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<T>>> GetAllAsync(int startIndex, int count, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<T>>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<T>>> GetAllAsync(int startIndex, int count, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
