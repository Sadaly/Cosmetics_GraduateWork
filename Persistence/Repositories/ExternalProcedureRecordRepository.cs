using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class ExternalProcedureRecordRepository(AppDbContext dbContext)
        : EntityWithTypeRepository<ExternalProcedureRecordType, ExternalProcedureRecord>(dbContext), IExternalProcedureRecordRepository
    {
        private protected override IQueryable<ExternalProcedureRecord> GetAllInclude()
            => base.GetAllInclude()
            .Include(e => e.PatientCard)
            .Include(e => e.Type);
    }
}
