using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ProcedureTypes.Queries.GetAll
{
	internal class ProcedureTypeGetAllQueryHandler(IProcedureTypeRepository procedureTypeRepository) : IQueryHandler<ProcedureTypeGetAllQuery, List<ProcedureTypeResponse>>
	{
		public async Task<Result<List<ProcedureTypeResponse>>> Handle(ProcedureTypeGetAllQuery request, CancellationToken cancellationToken)
		{
			var entities = request.StartIndex == null || request.Count == null
				? await procedureTypeRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
				: await procedureTypeRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
			if (entities.IsFailure) return Result.Failure<List<ProcedureTypeResponse>>(entities.Error);

			var listRes = entities.Value.Select(u => new ProcedureTypeResponse(u)).ToList();

			return listRes;
		}
	}
}
