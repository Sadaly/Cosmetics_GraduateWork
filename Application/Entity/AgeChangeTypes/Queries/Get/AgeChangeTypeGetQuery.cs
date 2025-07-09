using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.AgeChangeTypes.Queries.Get;
public sealed record AgeChangeTypeGetQuery(EntityQueries<AgeChangeType> Query) : IQuery<AgeChangeTypeResponse>;

