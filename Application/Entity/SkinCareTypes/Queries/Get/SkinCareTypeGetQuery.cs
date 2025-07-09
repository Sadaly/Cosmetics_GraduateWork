using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.SkinCareTypes.Queries.Get;
public sealed record SkinCareTypeGetQuery(EntityQueries<SkinCareType> Query) : IQuery<SkinCareTypeResponse>;

