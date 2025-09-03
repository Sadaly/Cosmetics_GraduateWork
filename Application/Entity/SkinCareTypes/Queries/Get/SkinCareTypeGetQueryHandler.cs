using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinCareTypes.Queries.Get
{
	internal class SkinCareTypeGetQueryHandler(ISkinCareTypeRepository skinCareTypeRepository) : IQueryHandler<SkinCareTypeGetQuery, SkinCareTypeResponse>
	{
		public async Task<Result<SkinCareTypeResponse>> Handle(SkinCareTypeGetQuery request, CancellationToken cancellationToken)
		{
			var entity = await skinCareTypeRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
			if (entity.IsFailure) return Result.Failure<SkinCareTypeResponse>(entity.Error);

			var response = new SkinCareTypeResponse(entity.Value);

			return response;
		}
	}
}
