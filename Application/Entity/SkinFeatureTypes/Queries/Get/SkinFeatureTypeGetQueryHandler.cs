using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinFeatureTypes.Queries.Get
{
	internal class SkinFeatureTypeGetQueryHandler(ISkinFeatureTypeRepository skinFeatureTypeRepository) : IQueryHandler<SkinFeatureTypeGetQuery, SkinFeatureTypeResponse>
	{
		public async Task<Result<SkinFeatureTypeResponse>> Handle(SkinFeatureTypeGetQuery request, CancellationToken cancellationToken)
		{
			var entity = await skinFeatureTypeRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
			if (entity.IsFailure) return Result.Failure<SkinFeatureTypeResponse>(entity.Error);

			var response = new SkinFeatureTypeResponse(entity.Value);

			return response;
		}
	}
}
