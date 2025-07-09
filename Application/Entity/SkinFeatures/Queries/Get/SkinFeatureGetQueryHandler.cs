using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinFeatures.Queries.Get
{
    internal class SkinFeatureGetQueryHandler(ISkinFeatureRepository SkinFeatureRepository) : IQueryHandler<SkinFeatureGetQuery, SkinFeatureResponse>
    {
        public async Task<Result<SkinFeatureResponse>> Handle(SkinFeatureGetQuery request, CancellationToken cancellationToken)
        {
            var entity = await SkinFeatureRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
            if (entity.IsFailure) return Result.Failure<SkinFeatureResponse>(entity.Error);

            var response = new SkinFeatureResponse(entity.Value);

            return response;
        }
    }
}
