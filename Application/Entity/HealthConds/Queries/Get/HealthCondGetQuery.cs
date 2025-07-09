using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.HealthConds.Queries.Get;
public sealed record HealthCondGetQuery(EntityQueries<HealthCond> Query) : IQuery<HealthCondResponse>;
