using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class PatientRepository(AppDbContext dbContext) : TRepository<Patient>(dbContext), IPatientRepository
    {
        //Todo: переделать путем выделения этой строки в отдельный метод в родительском классе и перезаписи его для всех других методов GetAll GetById и тд
        public override async Task<Result<Patient>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) return Result.Failure<Patient>(GetErrorIdEmpty());
            //  ↓↓↓↓    вот эта строка снизу    ↓↓↓↓
            var entity = await _dbSet.Where(e => !e.IsSoftDelete).Include(e => e.Card).FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
            if (entity == null) return Result.Failure<Patient>(GetErrorNotFound());
            if (entity.IsSoftDelete) return Result.Failure<Patient>(PersistenceErrors.Entity<Patient>.IsSoftDeleted);

            return Result.Success(entity);
        }
    }
}
