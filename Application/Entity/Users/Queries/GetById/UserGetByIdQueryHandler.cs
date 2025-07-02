using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Users.Queries.UserGetById
{
    internal sealed class UserGetByIdQueryHandler : IQueryHandler<UserGetByIdQuery, UserResponse>
    {
        private readonly IUserRepository _userRepository;

        public UserGetByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserResponse>> Handle(UserGetByIdQuery request, CancellationToken cancellationToken)
        {
            var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (userResult.IsFailure) return Result.Failure<UserResponse>(userResult.Error);

            var response = new UserResponse(userResult.Value);

            return response;
        }
    }
}
