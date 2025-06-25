using Domain.Abstractions;
using Domain.Common;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;

namespace Persistence.Abstractions
{
    public abstract class TRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        protected abstract Error GetErrorNotFound();
        protected abstract Error GetErrorIdEmpty();

        public TRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        protected abstract Task<Result<T>> VerificationBeforeAddingAsync(Result<T> entity, CancellationToken cancellationToken);
        protected abstract Task<Result<T>> VerificationBeforeUpdateAsync(Result<T> entity, CancellationToken cancellationToken);
        protected abstract Task<Result<T>> VerificationBeforeRemoveAsync(Result<T> entity, CancellationToken cancellationToken);

        public virtual async Task<Result<T>> AddAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            Result<T> verify = await VerificationBeforeAddingAsync(entity, cancellationToken);
            if (verify.IsFailure) return verify;

            await _dbSet.AddAsync(entity.Value, cancellationToken);
            return entity;
        }

        public async Task<Result<T>> GetFromDBAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) return Result.Failure<T>(GetErrorIdEmpty());

            T? entity = await _dbContext.Set<T>().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
            if (entity == null) return Result.Failure<T>(GetErrorNotFound());

            return Result.Success(entity);
        }               

        public async Task<Result<T>> GetFromDBAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            T? entity = await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
            if (entity == null) return Result.Failure<T>(GetErrorNotFound());
            return entity;
        }

        public Task<Result<T>> UpdateAsync(Result<T> entity, CancellationToken cancellationToken = default)
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
