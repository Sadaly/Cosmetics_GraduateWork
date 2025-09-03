using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
	public class SkinFeatureRepository(AppDbContext dbContext)
		: EntityWithTypeRepository<SkinFeatureType, SkinFeature>(dbContext), ISkinFeatureRepository
	{
		private protected override IQueryable<SkinFeature> GetAllInclude()
			=> base.GetAllInclude()
			.Include(e => e.PatientCard)
			.Include(e => e.Type);
	}
}
