using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.Users.Queries
{
	public sealed record UserQueries(Expression<Func<User, bool>> Predicate) : EntityQueries<User>(Predicate)
	{
		public static UserQueries GetByUsername(string Username)
		{
			return new UserQueries(x => x.Username.Value == Username);
		}
	}
}
