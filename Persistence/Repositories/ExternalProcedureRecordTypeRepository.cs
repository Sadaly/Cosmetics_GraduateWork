using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class ExternalProcedureRecordTypeRepository(AppDbContext dbContext, IEntityWithTypeRepository<ExternalProcedureRecordType, ExternalProcedureRecord> EntityWithTypeRepository) 
        : EntityTypeRepository<ExternalProcedureRecordType, ExternalProcedureRecord>(dbContext, EntityWithTypeRepository), IExternalProcedureRecordTypeRepository
    {
    }
}
