using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Doctors.Queries.Get;
public sealed record DoctorGetQuery(EntityQueries<Doctor> Query) : IQuery<DoctorResponse>;

