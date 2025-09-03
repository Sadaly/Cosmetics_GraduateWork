using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.AgeChanges.Queries.GetAll
{
	internal class AgeChangeGetAllQueryHandler(IAgeChangeRepository ageChangeRepository) : IQueryHandler<AgeChangeGetAllQuery, List<AgeChangeResponse>>
	{
		public async Task<Result<List<AgeChangeResponse>>> Handle(AgeChangeGetAllQuery request, CancellationToken cancellationToken)
		{
			var entities = request.StartIndex == null || request.Count == null
				? await ageChangeRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
				: await ageChangeRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
			if (entities.IsFailure) return Result.Failure<List<AgeChangeResponse>>(entities.Error);

			var listRes = entities.Value.Select(u => new AgeChangeResponse(u)).ToList();

			return listRes;
		}
	}
}
