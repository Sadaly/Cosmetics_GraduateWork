using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class PatientRepository(AppDbContext dbContext) 
        : TRepository<Patient>(dbContext), IPatientRepository
    {
        private protected override IQueryable<Patient> GetAllInclude()
            => base.GetAllInclude().Include(e => e.Card).Include(e => e.Card.SkinFeatures);
    }
}
