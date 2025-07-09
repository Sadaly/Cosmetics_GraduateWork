using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.HealthCondTypes.Queries.Get;
public sealed record HealthCondTypeGetQuery(EntityQueries<HealthCondType> Query) : IQuery<HealthCondTypeResponse>;

