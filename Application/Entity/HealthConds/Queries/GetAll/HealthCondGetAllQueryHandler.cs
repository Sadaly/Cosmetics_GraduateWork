using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.HealthConds.Queries.GetAll
{
    internal class HealthCondGetAllQueryHandler(IHealthCondRepository healthCondRepository) : IQueryHandler<HealthCondGetAllQuery, List<HealthCondResponse>>
    {
        public async Task<Result<List<HealthCondResponse>>> Handle(HealthCondGetAllQuery request, CancellationToken cancellationToken)
        {
            var entities = request.StartIndex == null || request.Count == null
                ? await healthCondRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
                : await healthCondRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
            if (entities.IsFailure) return Result.Failure<List<HealthCondResponse>>(entities.Error);

            var listRes = entities.Value.Select(u => new HealthCondResponse(u)).ToList();

            return listRes;
        }
    }
}
