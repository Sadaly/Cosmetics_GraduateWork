using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class PatientCardRepository(AppDbContext dbContext) : TRepository<PatientCard>(dbContext), IPatientCardRepository
    {
        private protected override IQueryable<PatientCard> GetInclude()
            => base.GetInclude().Include(e => e.Patient);
    }
}
