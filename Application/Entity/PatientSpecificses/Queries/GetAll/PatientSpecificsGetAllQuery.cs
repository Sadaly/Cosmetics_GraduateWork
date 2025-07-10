using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.PatientSpecificses.Queries.GetAll;
public sealed record PatientSpecificsGetAllQuery(
    EntityQueries<PatientSpecifics> Query,
    int? StartIndex = null,
    int? Count = null) : IQuery<List<PatientSpecificsResponse>>;