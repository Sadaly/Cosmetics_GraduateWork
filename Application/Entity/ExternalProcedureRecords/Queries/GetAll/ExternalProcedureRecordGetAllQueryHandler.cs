using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ExternalProcedureRecords.Queries.GetAll
{
	internal class ExternalProcedureRecordGetAllQueryHandler(IExternalProcedureRecordRepository externalProcedureRecordRepository) : IQueryHandler<ExternalProcedureRecordGetAllQuery, List<ExternalProcedureRecordResponse>>
	{
		public async Task<Result<List<ExternalProcedureRecordResponse>>> Handle(ExternalProcedureRecordGetAllQuery request, CancellationToken cancellationToken)
		{
			var entities = request.StartIndex == null || request.Count == null
				? await externalProcedureRecordRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
				: await externalProcedureRecordRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
			if (entities.IsFailure) return Result.Failure<List<ExternalProcedureRecordResponse>>(entities.Error);

			var listRes = entities.Value.Select(u => new ExternalProcedureRecordResponse(u)).ToList();

			return listRes;
		}
	}
}
