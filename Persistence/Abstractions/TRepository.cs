using Domain.Abstractions;
using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Abstractions
{
    public abstract class TRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        protected virtual Error GetErrorIdEmpty()
        {
            return PersistenceErrors.Entity<T>.IdEmpty;
        }

        protected virtual Error GetErrorNotFound()
        {
            return PersistenceErrors.Entity<T>.NotFound;
        }

        public TRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        protected abstract Task<Result<T>> VerificationBeforeAddingAsync(Result<T> entity, CancellationToken cancellationToken);

        protected virtual async Task<Result<T>> VerificationBeforeUpdateAsync(Result<T> entity, CancellationToken cancellationToken)
        {
            if (entity.IsFailure) return Result.Failure<T>(entity);
            return await GetByIdAsync(entity.Value.Id, cancellationToken);
        }
        protected virtual async Task<Result<T>> VerificationBeforeRemoveAsync(Result<T> entity, CancellationToken cancellationToken)
        {            
            if (entity.IsFailure) return Result.Failure<T>(entity);
            if (!entity.Value.IsSoftDelete) return Result.Failure<T>(PersistenceErrors.Entity<T>.ShouldBeSoftDeleted);
            return await GetByIdAsync(entity.Value.Id, cancellationToken);
        }

        public virtual async Task<Result<T>> AddAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            var verify = await VerificationBeforeAddingAsync(entity, cancellationToken);
            if (verify.IsFailure) return verify;

            await _dbSet.AddAsync(entity.Value, cancellationToken);
            return entity;
        }

        public async Task<Result<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) return Result.Failure<T>(GetErrorIdEmpty());

            T? entity = await _dbSet.AsNoTracking().Where(e => !e.IsSoftDelete).FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
            if (entity == null) return Result.Failure<T>(GetErrorNotFound());
            if (entity.IsSoftDelete) return Result.Failure<T>(PersistenceErrors.Entity<T>.IsSoftDeleted);

            return Result.Success(entity);
        }

        public async Task<Result<T>> GetByPredicateAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            T? entity = await _dbSet.AsNoTracking().Where(e => !e.IsSoftDelete).FirstOrDefaultAsync(predicate, cancellationToken);
            if (entity == null) return Result.Failure<T>(GetErrorNotFound());
            if (entity.IsSoftDelete) return Result.Failure<T>(PersistenceErrors.Entity<T>.IsSoftDeleted);

            return entity;
        }

        public async Task<Result<T>> UpdateAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            var result = await VerificationBeforeUpdateAsync(entity, cancellationToken);
            if (result.IsFailure) return Result.Failure<T>(result.Error);

            _dbContext.Entry(entity.Value).State = EntityState.Modified;

            return entity;
        }

        public async Task<Result<T>> RemoveAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            var result = await VerificationBeforeRemoveAsync(entity, cancellationToken);
            if (result.IsFailure) return Result.Failure<T>(result.Error);

            _dbContext.Entry(entity.Value).State = EntityState.Modified;

            return entity;
        }

        public async Task<Result<List<T>>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().Where(e => !e.IsSoftDelete).OrderBy(e => e.CreatedAt).ToListAsync(cancellationToken);
   

        public async Task<Result<List<T>>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().Where(e => !e.IsSoftDelete).Where(predicate).OrderBy(e => e.CreatedAt).ToListAsync(cancellationToken); 
        

        public async Task<Result<List<T>>> GetAllAsync(int startIndex, int count, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (count < 1) return Result.Failure<List<T>>(PersistenceErrors.IncorrectCount);
            if (startIndex < 0) return Result.Failure<List<T>>(PersistenceErrors.IncorrectStartIndex);

            var result = await _dbSet
                .AsNoTracking()
                .Where(e => !e.IsSoftDelete)
                .Where(predicate)
                .OrderBy(e => e.CreatedAt)
                .Skip(startIndex)
                .Take(count)
                .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<Result<List<T>>> GetAllAsync(int startIndex, int count, CancellationToken cancellationToken = default)
        {
            if (count < 1) return Result.Failure<List<T>>(PersistenceErrors.IncorrectCount);
            if (startIndex < 0) return Result.Failure<List<T>>(PersistenceErrors.IncorrectStartIndex);

            var result = await _dbSet
                .AsNoTracking()
                .Where(e => !e.IsSoftDelete)
                .OrderBy(e => e.CreatedAt)
                .Skip(startIndex)
                .Take(count)
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
