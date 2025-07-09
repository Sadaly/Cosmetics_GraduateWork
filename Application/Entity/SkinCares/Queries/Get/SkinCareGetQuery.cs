using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.SkinCares.Queries.Get;
public sealed record SkinCareGetQuery(EntityQueries<SkinCare> Query) : IQuery<SkinCareResponse>;
