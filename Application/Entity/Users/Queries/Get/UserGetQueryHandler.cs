using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Users.Queries.Get
{
	internal sealed class UserGetQueryHandler(IUserRepository userRepository) : IQueryHandler<UserGetQuery, UserResponse>
	{
		public async Task<Result<UserResponse>> Handle(UserGetQuery request, CancellationToken cancellationToken)
		{
			var userResult = await userRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
			if (userResult.IsFailure) return Result.Failure<UserResponse>(userResult.Error);

			var response = new UserResponse(userResult.Value);

			return response;
		}
	}
}
