using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Procedures.Queries.Get
{
	internal class ProcedureGetQueryHandler(IProcedureRepository procedureRepository) : IQueryHandler<ProcedureGetQuery, ProcedureResponse>
	{
		public async Task<Result<ProcedureResponse>> Handle(ProcedureGetQuery request, CancellationToken cancellationToken)
		{
			var entity = await procedureRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
			if (entity.IsFailure) return Result.Failure<ProcedureResponse>(entity.Error);

			var response = new ProcedureResponse(entity.Value);

			return response;
		}
	}
}
