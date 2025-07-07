using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.Patients.Queries
{
    public sealed record PatientQueries(Expression<Func<Patient, bool>> Predicate) : EntityQueries<Patient>(Predicate)
    {
    }
}