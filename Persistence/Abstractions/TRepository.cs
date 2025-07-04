using Domain.Abstractions;
using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace Persistence.Abstractions
{
    public abstract class TRepository<T>(AppDbContext dbContext) : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _dbContext = dbContext;
        protected readonly DbSet<T> _dbSet = dbContext.Set<T>();

        #region AddRemoveUpdate
        public virtual async Task<Result<T>> AddAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            var verify = await VerificationBeforeAddingAsync(entity, cancellationToken);
            if (verify.IsFailure) return verify;

            await _dbSet.AddAsync(entity.Value, cancellationToken);
            return entity;
        }
        public async Task<Result<T>> UpdateAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            var result = await VerificationBeforeUpdateAsync(entity, cancellationToken);
            if (result.IsFailure) return Result.Failure<T>(result.Error);

            _dbContext.Entry(entity.Value).State = EntityState.Modified;

            return entity;
        }
        public virtual async Task<Result<T>> RemoveAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            var result = await VerificationBeforeRemoveAsync(entity, cancellationToken);
            if (result.IsFailure) return Result.Failure<T>(result.Error);

            entity.Value.SoftDelete();

            _dbContext.Entry(entity.Value).State = EntityState.Modified;

            return entity;
        }
        #endregion
        #region Errors
        protected virtual Error GetErrorIdEmpty()
        {
            return PersistenceErrors.Entity<T>.IdEmpty;
        }

        protected virtual Error GetErrorNotFound()
        {
            return PersistenceErrors.Entity<T>.NotFound;
        }
        #endregion
        #region Get
        public virtual async Task<Result<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, FetchMode mode = FetchMode.Default)
        {
            if (id == Guid.Empty) return Result.Failure<T>(GetErrorIdEmpty());
            var entity = await GetByModeAsync(e => e.Id == id, cancellationToken, mode);
            if (entity == null) return Result.Failure<T>(GetErrorNotFound());
            if (entity.IsSoftDelete) return Result.Failure<T>(PersistenceErrors.Entity<T>.IsSoftDeleted);

            return Result.Success(entity);
        }
        public async Task<Result<T>> GetByPredicateAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, FetchMode mode = FetchMode.Default)
        {
            var entity = await GetByModeAsync(predicate, cancellationToken, mode);
            if (entity == null) return Result.Failure<T>(GetErrorNotFound());
            if (entity.IsSoftDelete) return Result.Failure<T>(PersistenceErrors.Entity<T>.IsSoftDeleted);

            return entity;
        }
        public async Task<Result<List<T>>> GetAllAsync(CancellationToken cancellationToken = default, FetchMode mode = FetchMode.Default)
            => await GetAllByModeAsync(0, _dbSet.Count(), x => true, cancellationToken, mode);

        public async Task<Result<List<T>>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, FetchMode mode = FetchMode.Default)
            => await GetAllByModeAsync(0, _dbSet.Count(), predicate, cancellationToken, mode);

        public async Task<Result<List<T>>> GetAllAsync(int startIndex, int count, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, FetchMode mode = FetchMode.Default) {
            if (count < 1) return Result.Failure<List<T>>(PersistenceErrors.IncorrectCount);
            if (startIndex < 0) return Result.Failure<List<T>>(PersistenceErrors.IncorrectStartIndex);
            return await GetAllByModeAsync(startIndex, count, predicate, cancellationToken, mode);
        }

        public async Task<Result<List<T>>> GetAllAsync(int startIndex, int count, CancellationToken cancellationToken = default, FetchMode mode = FetchMode.Default) {
            if (count < 1) return Result.Failure<List<T>>(PersistenceErrors.IncorrectCount);
            if (startIndex < 0) return Result.Failure<List<T>>(PersistenceErrors.IncorrectStartIndex);
            return await GetAllByModeAsync(startIndex, count, x => true, cancellationToken, mode);
        }

        /// <summary>
        /// Метод возвращает список сущностей по предикату и может быть переопределен в наследуемом классе
        /// </summary>
        /// <returns></returns>
        private protected virtual IQueryable<T> GetAllDefault()
            => _dbSet.Where(e => !e.IsSoftDelete);

        /// <summary>
        /// Метод возвращает список сущностей по предикату с методом AsNoTracking и может быть переопределен в наследуемом классе
        /// </summary>
        /// <returns></returns>
        private protected virtual IQueryable<T> GetAllNoTracking()
            => _dbSet.AsNoTracking().Where(e => !e.IsSoftDelete);

        /// <summary>
        /// Метод возвращает список сущностей по предикату с методом Include и, если вызывается, должее быть переопределен в наследуемом классе
        /// </summary>
        /// <returns></returns>
        private protected virtual IQueryable<T> GetAllInclude()
            => _dbSet.Where(e => !e.IsSoftDelete);

        /// <summary>
        /// Метод возвращает список сущностей по предикату с методом Include и AsNoTracking и, если вызывается, должее быть переопределен в наследуемом классе
        /// </summary>
        /// <returns></returns>
        private protected virtual IQueryable<T> GetAllIncludeNoTracking()
            => _dbSet.Where(e => !e.IsSoftDelete);

        private async Task<T?> GetByModeAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken, FetchMode mode)
        {
            var entities = mode switch
            {
                FetchMode.Default => GetAllDefault(),
                FetchMode.NoTracking => GetAllNoTracking(),
                FetchMode.Include => GetAllInclude(),
                FetchMode.IncludeNoTracking => GetAllIncludeNoTracking(),
                _ => null
            };
            if (entities != null) return await entities.FirstOrDefaultAsync(predicate, cancellationToken);
            return null;
        }
        private async Task<List<T>?> GetAllByModeAsync(int startIndex, int count, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken, FetchMode mode)
        {
            var entities = mode switch
            {
                FetchMode.Default => GetAllDefault(),
                FetchMode.NoTracking => GetAllNoTracking(),
                FetchMode.Include => GetAllInclude(),
                FetchMode.IncludeNoTracking => GetAllIncludeNoTracking(),
                _ => null
            };
            if (entities != null) return await entities.Where(predicate).OrderBy(e => e.CreatedAt).Skip(startIndex).Take(count).ToListAsync(cancellationToken);
            return null;
        }
        #endregion
        #region Verification
        protected virtual async Task<Result<T>> VerificationBeforeAddingAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            if (entity.IsFailure) return Result.Failure<T>(entity);
            var exists = await GetByIdAsync(entity.Value.Id, cancellationToken);
            if (exists.IsSuccess) return Result.Failure<T>(PersistenceErrors.Entity<T>.AlreadyExists);
            return entity;
        }
        protected virtual async Task<Result<T>> VerificationBeforeUpdateAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            if (entity.IsFailure) return Result.Failure<T>(entity);
            return await GetByIdAsync(entity.Value.Id, cancellationToken);
        }
        protected virtual async Task<Result<T>> VerificationBeforeRemoveAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            if (entity.IsFailure) return Result.Failure<T>(entity);
            if (entity.Value.IsSoftDelete) return Result.Failure<T>(PersistenceErrors.Entity<T>.IsSoftDeleted);
            return await GetByIdAsync(entity.Value.Id, cancellationToken);
        }
        #endregion
    }
}
