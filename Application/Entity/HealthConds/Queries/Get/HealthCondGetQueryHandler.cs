using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.HealthConds.Queries.Get
{
    internal class HealthCondGetQueryHandler(IHealthCondRepository healthCondRepository) : IQueryHandler<HealthCondGetQuery, HealthCondResponse>
    {
        public async Task<Result<HealthCondResponse>> Handle(HealthCondGetQuery request, CancellationToken cancellationToken)
        {
            var entity = await healthCondRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
            if (entity.IsFailure) return Result.Failure<HealthCondResponse>(entity.Error);

            var response = new HealthCondResponse(entity.Value);

            return response;
        }
    }
}
