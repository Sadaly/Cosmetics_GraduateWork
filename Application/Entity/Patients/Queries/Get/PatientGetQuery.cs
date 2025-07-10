using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Patients.Queries.Get;
public sealed record PatientGetQuery(EntityQueries<Patient> Query) : IQuery<PatientResponse>;

