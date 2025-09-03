using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.AgeChangeTypes.Queries.GetAll
{
	internal class AgeChangeTypeGetAllQueryHandler(IAgeChangeTypeRepository ageChangeTypeRepository) : IQueryHandler<AgeChangeTypeGetAllQuery, List<AgeChangeTypeResponse>>
	{
		public async Task<Result<List<AgeChangeTypeResponse>>> Handle(AgeChangeTypeGetAllQuery request, CancellationToken cancellationToken)
		{
			var entities = request.StartIndex == null || request.Count == null
				? await ageChangeTypeRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
				: await ageChangeTypeRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
			if (entities.IsFailure) return Result.Failure<List<AgeChangeTypeResponse>>(entities.Error);

			var listRes = entities.Value.Select(u => new AgeChangeTypeResponse(u)).ToList();

			return listRes;
		}
	}
}
