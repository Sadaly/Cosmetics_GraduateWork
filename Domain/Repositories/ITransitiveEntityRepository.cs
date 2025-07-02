using Domain.Abstractions;
using Domain.Common;

namespace Domain.Repositories
{
    public interface ITransitiveEntityRepository<T> : IRepository<T> where T : TransitiveEntity<TypeEntity>
    {
    }
}
