using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class PatientSpecificsRepository(AppDbContext dbContext)
        : TRepository<PatientSpecifics>(dbContext), IPatientSpecificsRepository
    {
        private protected override IQueryable<PatientSpecifics> GetAllInclude()
            => base.GetAllInclude()
            .Include(e => e.PatientCard);
    }
}
