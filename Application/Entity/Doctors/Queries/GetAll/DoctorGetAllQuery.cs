using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Doctors.Queries.GetAll;
public sealed record DoctorGetAllQuery(
    EntityQueries<Doctor> Query,
    int? StartIndex = null,
    int? Count = null) : IQuery<List<DoctorResponse>>;