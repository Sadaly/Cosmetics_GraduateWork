using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ExternalProcedureRecordTypes.Queries.Get
{
    internal class ExternalProcedureRecordTypeGetQueryHandler(IExternalProcedureRecordTypeRepository externalProcedureRecordTypeRepository) : IQueryHandler<ExternalProcedureRecordTypeGetQuery, ExternalProcedureRecordTypeResponse>
    {
        public async Task<Result<ExternalProcedureRecordTypeResponse>> Handle(ExternalProcedureRecordTypeGetQuery request, CancellationToken cancellationToken)
        {
            var entity = await externalProcedureRecordTypeRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
            if (entity.IsFailure) return Result.Failure<ExternalProcedureRecordTypeResponse>(entity.Error);

            var response = new ExternalProcedureRecordTypeResponse(entity.Value);

            return response;
        }
    }
}
