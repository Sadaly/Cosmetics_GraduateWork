using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.PatientCards.Queries;

public sealed record PatientCardQueries(Expression<Func<PatientCard, bool>> Predicate) : EntityQueries<PatientCard>(Predicate)
{
}
