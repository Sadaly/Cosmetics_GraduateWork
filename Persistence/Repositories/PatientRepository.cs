using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class PatientRepository(AppDbContext dbContext) : TRepository<Patient>(dbContext), IPatientRepository
    {
        private protected override IQueryable<Patient> GetInclude()
            => base.GetInclude().Include(e => e.Card);
        private protected override IQueryable<Patient> GetAllInclude(int startIndex, int count, Expression<Func<Patient, bool>> predicate)
            => base.GetAllInclude(startIndex, count, predicate).Include(e => e.Card);
    }
}
