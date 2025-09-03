using Domain.Abstractions;
using Domain.Common;
using System.Linq.Expressions;

namespace Application.Abstractions
{
	public record EntityQueries<T>(Expression<Func<T, bool>> Predicate)
		where T : BaseEntity
	{
		public static EntityQueries<T> GetById(Guid Id)
		{
			return new EntityQueries<T>(x => x.Id == Id);
		}
		public static EntityQueries<T> GetByFilter(IEntityFilter<T> entityFilter)
		{
			var Predicate = entityFilter.ToPredicate();
			if (Predicate == null) return GetWithoutPredicate();
			return new EntityQueries<T>(Predicate);
		}
		public static EntityQueries<T> GetWithoutPredicate()
		{
			return new EntityQueries<T>(x => true);
		}
	}
}
