using Domain.Common;
using System.Linq.Expressions;

namespace Domain.Abstractions
{
	public interface IEntityFilter<TEntity> where TEntity : BaseEntity
	{
		Expression<Func<TEntity, bool>> ToPredicate();
	}
}