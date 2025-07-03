using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Users.Queries.GetById
{
    internal sealed class UserGetByIdQueryHandler(IUserRepository userRepository) : IQueryHandler<UserGetByIdQuery, UserResponse>
    {
        public async Task<Result<UserResponse>> Handle(UserGetByIdQuery request, CancellationToken cancellationToken)
        {
            var userResult = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (userResult.IsFailure) return Result.Failure<UserResponse>(userResult.Error);

            var response = new UserResponse(userResult.Value);

            return response;
        }
    }
}
