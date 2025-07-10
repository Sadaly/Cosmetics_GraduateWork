using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.Doctors.Queries;

public sealed record DoctorQueries(Expression<Func<Doctor, bool>> Predicate) : EntityQueries<Doctor>(Predicate)
{
}
