using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinFeatureTypes.Queries.GetAll
{
	internal class SkinFeatureTypeGetAllQueryHandler(ISkinFeatureTypeRepository skinFeatureTypeRepository) : IQueryHandler<SkinFeatureTypeGetAllQuery, List<SkinFeatureTypeResponse>>
	{
		public async Task<Result<List<SkinFeatureTypeResponse>>> Handle(SkinFeatureTypeGetAllQuery request, CancellationToken cancellationToken)
		{
			var entities = request.StartIndex == null || request.Count == null
				? await skinFeatureTypeRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
				: await skinFeatureTypeRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
			if (entities.IsFailure) return Result.Failure<List<SkinFeatureTypeResponse>>(entities.Error);

			var listRes = entities.Value.Select(u => new SkinFeatureTypeResponse(u)).ToList();

			return listRes;
		}
	}
}
