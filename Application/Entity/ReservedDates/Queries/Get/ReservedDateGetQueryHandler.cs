using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ReservedDates.Queries.Get
{
	internal class ReservedDateGetQueryHandler(IReservedDateRepository reservedDateRepository) : IQueryHandler<ReservedDateGetQuery, ReservedDateResponse>
	{
		public async Task<Result<ReservedDateResponse>> Handle(ReservedDateGetQuery request, CancellationToken cancellationToken)
		{
			var entity = await reservedDateRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
			if (entity.IsFailure) return Result.Failure<ReservedDateResponse>(entity.Error);

			var response = new ReservedDateResponse(entity.Value);

			return response;
		}
	}
}
