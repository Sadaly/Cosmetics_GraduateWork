using System.Linq.Expressions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Patients.Queries.GetAll;

public sealed record PatientsGetAllQuery(
    Expression<Func<Patient, bool>>? Predicate) : IQuery<List<PatientResponses>>;