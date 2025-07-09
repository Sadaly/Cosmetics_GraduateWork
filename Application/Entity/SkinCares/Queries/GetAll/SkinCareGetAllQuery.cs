using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.SkinCares.Queries.GetAll;
public sealed record SkinCareGetAllQuery(
    EntityQueries<SkinCare> Query,
    int? StartIndex = null,
    int? Count = null) : IQuery<List<SkinCareResponse>>;