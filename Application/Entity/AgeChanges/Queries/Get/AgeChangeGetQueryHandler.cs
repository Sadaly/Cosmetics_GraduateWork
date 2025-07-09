using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.AgeChanges.Queries.Get
{
    internal class AgeChangeGetQueryHandler(IAgeChangeRepository ageChangeRepository) : IQueryHandler<AgeChangeGetQuery, AgeChangeResponse>
    {
        public async Task<Result<AgeChangeResponse>> Handle(AgeChangeGetQuery request, CancellationToken cancellationToken)
        {
            var entity = await ageChangeRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
            if (entity.IsFailure) return Result.Failure<AgeChangeResponse>(entity.Error);

            var response = new AgeChangeResponse(entity.Value);

            return response;
        }
    }
}
