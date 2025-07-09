using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinCares.Queries.GetAll
{
    internal class SkinCareGetAllQueryHandler(ISkinCareRepository skinCareRepository) : IQueryHandler<SkinCareGetAllQuery, List<SkinCareResponse>>
    {
        public async Task<Result<List<SkinCareResponse>>> Handle(SkinCareGetAllQuery request, CancellationToken cancellationToken)
        {
            var entities = request.StartIndex == null || request.Count == null
                ? await skinCareRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
                : await skinCareRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
            if (entities.IsFailure) return Result.Failure<List<SkinCareResponse>>(entities.Error);

            var listRes = entities.Value.Select(u => new SkinCareResponse(u)).ToList();

            return listRes;
        }
    }
}
