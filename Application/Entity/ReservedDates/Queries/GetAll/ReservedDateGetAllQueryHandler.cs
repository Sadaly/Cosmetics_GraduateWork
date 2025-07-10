using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ReservedDates.Queries.GetAll
{
    internal class ReservedDateGetAllQueryHandler(IReservedDateRepository reservedDateRepository) : IQueryHandler<ReservedDateGetAllQuery, List<ReservedDateResponse>>
    {
        public async Task<Result<List<ReservedDateResponse>>> Handle(ReservedDateGetAllQuery request, CancellationToken cancellationToken)
        {
            var entities = request.StartIndex == null || request.Count == null
                ? await reservedDateRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
                : await reservedDateRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
            if (entities.IsFailure) return Result.Failure<List<ReservedDateResponse>>(entities.Error);

            var listRes = entities.Value.Select(u => new ReservedDateResponse(u)).ToList();

            return listRes;
        }
    }
}
