using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.AgeChangeTypes.Queries.Get
{
    internal class AgeChangeTypeGetQueryHandler(IAgeChangeTypeRepository ageChangeTypeRepository) : IQueryHandler<AgeChangeTypeGetQuery, AgeChangeTypeResponse>
    {
        public async Task<Result<AgeChangeTypeResponse>> Handle(AgeChangeTypeGetQuery request, CancellationToken cancellationToken)
        {
            var entity = await ageChangeTypeRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
            if (entity.IsFailure) return Result.Failure<AgeChangeTypeResponse>(entity.Error);

            var response = new AgeChangeTypeResponse(entity.Value);

            return response;
        }
    }
}
