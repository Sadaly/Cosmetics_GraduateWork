using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class HealthCondRepository(AppDbContext dbContext) 
        : EntityWithTypeRepository<HealthCondType, HealthCond>(dbContext), IHealthCondRepository
    {
        private protected override IQueryable<HealthCond> GetAllInclude()
            => base.GetAllInclude()
            .Include(e => e.PatientCard)
            .Include(e => e.Type);
    }
}
