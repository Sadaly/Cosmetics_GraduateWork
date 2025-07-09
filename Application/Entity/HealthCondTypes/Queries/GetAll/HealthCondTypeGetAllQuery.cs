using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.HealthCondTypes.Queries.GetAll;
public sealed record HealthCondTypeGetAllQuery(
    EntityQueries<HealthCondType> Query,
    int? StartIndex = null,
    int? Count = null) : IQuery<List<HealthCondTypeResponse>>;