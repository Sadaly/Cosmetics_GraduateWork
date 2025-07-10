using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.PatientCards.Queries.Get;
public sealed record PatientCardGetQuery(EntityQueries<PatientCard> Query) : IQuery<PatientCardResponse>;

