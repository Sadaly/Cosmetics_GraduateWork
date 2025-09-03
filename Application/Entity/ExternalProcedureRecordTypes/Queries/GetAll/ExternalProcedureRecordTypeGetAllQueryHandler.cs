using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ExternalProcedureRecordTypes.Queries.GetAll
{
	internal class ExternalProcedureRecordTypeGetAllQueryHandler(IExternalProcedureRecordTypeRepository externalProcedureRecordTypeRepository) : IQueryHandler<ExternalProcedureRecordTypeGetAllQuery, List<ExternalProcedureRecordTypeResponse>>
	{
		public async Task<Result<List<ExternalProcedureRecordTypeResponse>>> Handle(ExternalProcedureRecordTypeGetAllQuery request, CancellationToken cancellationToken)
		{
			var entities = request.StartIndex == null || request.Count == null
				? await externalProcedureRecordTypeRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
				: await externalProcedureRecordTypeRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
			if (entities.IsFailure) return Result.Failure<List<ExternalProcedureRecordTypeResponse>>(entities.Error);

			var listRes = entities.Value.Select(u => new ExternalProcedureRecordTypeResponse(u)).ToList();

			return listRes;
		}
	}
}
