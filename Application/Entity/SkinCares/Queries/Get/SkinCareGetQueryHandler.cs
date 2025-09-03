using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinCares.Queries.Get
{
	internal class SkinCareGetQueryHandler(ISkinCareRepository skinCareRepository) : IQueryHandler<SkinCareGetQuery, SkinCareResponse>
	{
		public async Task<Result<SkinCareResponse>> Handle(SkinCareGetQuery request, CancellationToken cancellationToken)
		{
			var entity = await skinCareRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
			if (entity.IsFailure) return Result.Failure<SkinCareResponse>(entity.Error);

			var response = new SkinCareResponse(entity.Value);

			return response;
		}
	}
}
