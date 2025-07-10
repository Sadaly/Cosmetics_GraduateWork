using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.PatientSpecificses.Queries.Get;
public sealed record PatientSpecificsGetQuery(EntityQueries<PatientSpecifics> Query) : IQuery<PatientSpecificsResponse>;
