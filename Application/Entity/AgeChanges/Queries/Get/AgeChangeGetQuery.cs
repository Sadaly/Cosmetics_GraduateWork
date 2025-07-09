using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.AgeChanges.Queries.Get;
public sealed record AgeChangeGetQuery(EntityQueries<AgeChange> Query) : IQuery<AgeChangeResponse>;
