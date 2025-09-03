using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinCareTypes.Queries.GetAll
{
	internal class SkinCareTypeGetAllQueryHandler(ISkinCareTypeRepository skinCareTypeRepository) : IQueryHandler<SkinCareTypeGetAllQuery, List<SkinCareTypeResponse>>
	{
		public async Task<Result<List<SkinCareTypeResponse>>> Handle(SkinCareTypeGetAllQuery request, CancellationToken cancellationToken)
		{
			var entities = request.StartIndex == null || request.Count == null
				? await skinCareTypeRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
				: await skinCareTypeRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
			if (entities.IsFailure) return Result.Failure<List<SkinCareTypeResponse>>(entities.Error);

			var listRes = entities.Value.Select(u => new SkinCareTypeResponse(u)).ToList();

			return listRes;
		}
	}
}
