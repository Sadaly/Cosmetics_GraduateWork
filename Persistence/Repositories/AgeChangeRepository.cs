using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
	public class AgeChangeRepository(AppDbContext dbContext)
		: EntityWithTypeRepository<AgeChangeType, AgeChange>(dbContext), IAgeChangeRepository
	{
		private protected override IQueryable<AgeChange> GetAllInclude()
			=> base.GetAllInclude()
			.Include(e => e.PatientCard)
			.Include(e => e.Type);
	}
}
