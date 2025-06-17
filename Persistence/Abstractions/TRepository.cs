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

        protected abstract Error GetErrorNotFound();
        protected abstract Error GetErrorIdEmpty();

        public TRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        protected abstract Task<Result> VerificationBeforeAddingAsync(Result<T> entity, CancellationToken cancellationToken);
        protected abstract Task<Result> VerificationBeforeUpdateAsync(Result<T> entity, CancellationToken cancellationToken);
        protected abstract Task<Result> VerificationBeforeRemoveAsync(Result<T> entity, CancellationToken cancellationToken);

        public virtual async Task<Result> AddAsync(Result<T> entity, CancellationToken cancellationToken = default)
        {
            Result result = await VerificationBeforeAddingAsync(entity, cancellationToken);
            if (result.IsFailure) return result;


            await _dbSet.AddAsync(entity.Value, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        /// <summary>
        /// Получение сущности типа репозитория по Id с учетом ошибки
        /// </summary>
        protected async Task<Result<T>> GetFromDBAsync(
            Guid id,
            CancellationToken cancellationToken
            ) => await GetFromDBAsync<T>(
                id,
                GetErrorIdEmpty(),
                GetErrorNotFound(),
                cancellationToken);

        /// <summary>
        /// Получение сущности по Id
        /// </summary>
        private async Task<TBaseEntity?> GetFromDBAsync<TBaseEntity>(
            Guid id,
            CancellationToken cancellationToken
            ) where TBaseEntity : BaseEntity =>
                await _dbContext
                    .Set<TBaseEntity>()
                    .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
        /// <summary>
        /// Получение результата поиска по Id с учетом ошибки 
        /// </summary>
        protected async Task<Result<TBaseEntity>> GetFromDBAsync<TBaseEntity>(
            Guid id,
            Error IdEmpty,
            Error NotFound,
            CancellationToken cancellationToken
            ) where TBaseEntity : BaseEntity
        {
            if (id == Guid.Empty) return Result.Failure<TBaseEntity>(IdEmpty);

            TBaseEntity? entity = await GetFromDBAsync<TBaseEntity>(id, cancellationToken);
            if (entity == null) return Result.Failure<TBaseEntity>(NotFound);

            return Result.Success(entity);
        }
        /// <summary>
        /// Получение сущности типа репозитория по предикату
        /// </summary>
        private async Task<T?> GetFromDBAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken
            ) => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        /// <summary>
        /// Получение сущности типа репозитория по предикату с учетом ошибки
        /// </summary>
        protected async Task<Result<T>> GetFromDBAsync(
            Expression<Func<T, bool>> predicate,
            Error NotFound,
            CancellationToken cancellationToken
            )
        {
            T? entity = await GetFromDBAsync(predicate, cancellationToken);
            if (entity == null) return Result.Failure<T>(NotFound);

            return Result.Success(entity);
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
