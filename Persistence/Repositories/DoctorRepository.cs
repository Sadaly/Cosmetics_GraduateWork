using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
	public class DoctorRepository(AppDbContext dbContext,
		IProcedureRepository procedureRepository)
		: TRepository<Doctor>(dbContext), IDoctorRepository
	{
		protected readonly IProcedureRepository _procedureRepository = procedureRepository;
		private protected override IQueryable<Doctor> GetAllInclude()
			=> base.GetAllInclude()
			.Include(e => e.Procedures);

		public override async Task<Result<Doctor>> RemoveAsync(Result<Doctor> entity, CancellationToken cancellationToken = default)
		{
			if (entity.IsFailure) return entity;
			var eList = await _procedureRepository.GetAllAsync(x => x.DoctorId == entity.Value.Id, cancellationToken);

			if (eList.IsFailure) return Result.Failure<Doctor>(eList.Error);

			foreach (var e in eList.Value)
			{
				e.RemoveDoctor();
				var update = await _procedureRepository.UpdateAsync(e, cancellationToken);
				if (update.IsFailure) return Result.Failure<Doctor>(update.Error);
			}
			return await base.RemoveAsync(entity, cancellationToken);
		}
	}
}
