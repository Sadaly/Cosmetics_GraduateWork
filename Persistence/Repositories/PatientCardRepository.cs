using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class PatientCardRepository(AppDbContext dbContext) 
        : TRepository<PatientCard>(dbContext), IPatientCardRepository
    {
        private protected override IQueryable<PatientCard> GetAllInclude()
            => base.GetAllInclude()
            .Include(e => e.AgeChanges)
            .Include(e => e.ExternalProcedureRecords)
            .Include(e => e.HealthConds)
            .Include(e => e.Patient)
            .Include(e => e.Procedures)
            .Include(e => e.SkinCares)
            .Include(e => e.SkinFeatures)
            .Include(e => e.Specifics);
    }
}
