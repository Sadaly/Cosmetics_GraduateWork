using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Patients.Queries.GetAll;

public sealed record PatientGetAllQuery(
	EntityQueries<Patient> Query,
	int? StartIndex = null,
	int? Count = null) : IQuery<List<PatientResponse>>;