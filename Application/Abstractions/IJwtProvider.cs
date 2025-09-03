using Domain.Entity;

namespace Application.Abstractions
{
	public interface IJwtProvider
	{
		string Generate(User user);
	}
}
