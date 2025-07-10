using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.PatientSpecificses.Queries;

public sealed record PatientSpecificsQueries(Expression<Func<PatientSpecifics, bool>> Predicate) : EntityQueries<PatientSpecifics>(Predicate)
{
}
