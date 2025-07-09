using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ProcedureTypes.Queries.Get
{
    internal class ProcedureTypeGetQueryHandler(IProcedureTypeRepository procedureTypeRepository) : IQueryHandler<ProcedureTypeGetQuery, ProcedureTypeResponse>
    {
        public async Task<Result<ProcedureTypeResponse>> Handle(ProcedureTypeGetQuery request, CancellationToken cancellationToken)
        {
            var entity = await procedureTypeRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
            if (entity.IsFailure) return Result.Failure<ProcedureTypeResponse>(entity.Error);

            var response = new ProcedureTypeResponse(entity.Value);

            return response;
        }
    }
}
