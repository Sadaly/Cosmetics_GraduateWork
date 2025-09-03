using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinFeatures.Queries.GetAll
{
	internal class SkinFeatureGetAllQueryHandler(ISkinFeatureRepository skinFeatureRepository) : IQueryHandler<SkinFeatureGetAllQuery, List<SkinFeatureResponse>>
	{
		public async Task<Result<List<SkinFeatureResponse>>> Handle(SkinFeatureGetAllQuery request, CancellationToken cancellationToken)
		{
			var entities = request.StartIndex == null || request.Count == null
				? await skinFeatureRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
				: await skinFeatureRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
			if (entities.IsFailure) return Result.Failure<List<SkinFeatureResponse>>(entities.Error);

			var listRes = entities.Value.Select(u => new SkinFeatureResponse(u)).ToList();

			return listRes;
		}
	}
}
