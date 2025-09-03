using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Procedures.Queries.GetAll
{
	internal class ProcedureGetAllQueryHandler(IProcedureRepository procedureRepository) : IQueryHandler<ProcedureGetAllQuery, List<ProcedureResponse>>
	{
		public async Task<Result<List<ProcedureResponse>>> Handle(ProcedureGetAllQuery request, CancellationToken cancellationToken)
		{
			var entities = request.StartIndex == null || request.Count == null
				? await procedureRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
				: await procedureRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
			if (entities.IsFailure) return Result.Failure<List<ProcedureResponse>>(entities.Error);

			var listRes = entities.Value.Select(u => new ProcedureResponse(u)).ToList();

			return listRes;
		}
	}
}
