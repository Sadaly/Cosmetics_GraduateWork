using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class ProcedureRepository(AppDbContext dbContext) 
        : EntityWithTypeRepository<ProcedureType, Procedure>(dbContext), IProcedureRepository
    {
        private protected override IQueryable<Procedure> GetAllInclude()
            => base.GetAllInclude()
            .Include(e => e.Doctor)
            .Include(e => e.Notification)
            .Include(e => e.PatientCard)
            .Include(e => e.Type);
    }
}
