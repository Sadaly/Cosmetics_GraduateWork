using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Patients.Queries.GetAll;

public sealed record PatientsGetAllQuery(
    EntityQueries<Patient> Query,
    int? StartIndex = null,
    int? Count = null) : IQuery<List<PatientResponse>>;