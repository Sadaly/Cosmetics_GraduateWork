using Domain.Entity;

namespace Application.Entity.Users.Queries
{
	public sealed record UserResponse(
	Guid UserId,
	string Username,
	string Email,
	DateTime RegistrationDate,
	DateTime UpdateDate)
	{
		internal UserResponse(User user)
			: this(user.Id, user.Username.Value, user.Email.Value,
				  user.CreatedAt, user.UpdatedAt)
		{ }
	}
}
