using Domain.Common;
using WebApi.Abstractions;

namespace WebApi.SupportData
{
    public sealed record TakeData<TFilter, TEntity>(
        int StartIndex,
        int Count,
        TFilter? Filter)
        where TFilter : IEntityFilter<TEntity>
        where TEntity : BaseEntity;
}
