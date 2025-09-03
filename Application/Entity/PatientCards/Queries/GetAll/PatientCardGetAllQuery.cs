using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.PatientCards.Queries.GetAll;
public sealed record PatientCardGetAllQuery(
	EntityQueries<PatientCard> Query,
	int? StartIndex = null,
	int? Count = null) : IQuery<List<PatientCardResponse>>;