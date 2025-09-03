using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ExternalProcedureRecords.Queries.Get
{
	internal class ExternalProcedureRecordGetQueryHandler(IExternalProcedureRecordRepository externalProcedureRecordRepository) : IQueryHandler<ExternalProcedureRecordGetQuery, ExternalProcedureRecordResponse>
	{
		public async Task<Result<ExternalProcedureRecordResponse>> Handle(ExternalProcedureRecordGetQuery request, CancellationToken cancellationToken)
		{
			var entity = await externalProcedureRecordRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
			if (entity.IsFailure) return Result.Failure<ExternalProcedureRecordResponse>(entity.Error);

			var response = new ExternalProcedureRecordResponse(entity.Value);

			return response;
		}
	}
}
