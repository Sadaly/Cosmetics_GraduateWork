using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.HealthCondTypes.Queries.Get
{
	internal class HealthCondTypeGetQueryHandler(IHealthCondTypeRepository healthCondTypeRepository) : IQueryHandler<HealthCondTypeGetQuery, HealthCondTypeResponse>
	{
		public async Task<Result<HealthCondTypeResponse>> Handle(HealthCondTypeGetQuery request, CancellationToken cancellationToken)
		{
			var entity = await healthCondTypeRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
			if (entity.IsFailure) return Result.Failure<HealthCondTypeResponse>(entity.Error);

			var response = new HealthCondTypeResponse(entity.Value);

			return response;
		}
	}
}
