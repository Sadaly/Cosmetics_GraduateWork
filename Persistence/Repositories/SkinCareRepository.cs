using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class SkinCareRepository(AppDbContext dbContext)
        : EntityWithTypeRepository<SkinCareType, SkinCare>(dbContext), ISkinCareRepository
    {
        private protected override IQueryable<SkinCare> GetAllInclude()
            => base.GetAllInclude()
            .Include(e => e.PatientCard)
            .Include(e => e.Type);
    }
}
