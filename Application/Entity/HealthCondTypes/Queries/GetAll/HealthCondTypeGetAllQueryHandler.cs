using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.HealthCondTypes.Queries.GetAll
{
    internal class HealthCondTypeGetAllQueryHandler(IHealthCondTypeRepository healthCondTypeRepository) : IQueryHandler<HealthCondTypeGetAllQuery, List<HealthCondTypeResponse>>
    {
        public async Task<Result<List<HealthCondTypeResponse>>> Handle(HealthCondTypeGetAllQuery request, CancellationToken cancellationToken)
        {
            var entities = request.StartIndex == null || request.Count == null
                ? await healthCondTypeRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
                : await healthCondTypeRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
            if (entities.IsFailure) return Result.Failure<List<HealthCondTypeResponse>>(entities.Error);

            var listRes = entities.Value.Select(u => new HealthCondTypeResponse(u)).ToList();

            return listRes;
        }
    }
}
