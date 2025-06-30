using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;
namespace Application.Entity.Users.Queries.TakeUsers
{
    internal class UsersTakeQueryHandler : IQueryHandler<UsersTakeQuery, List<UserResponse>>
    {
        private readonly IUserRepository _userRepository;

        public UsersTakeQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<List<UserResponse>>> Handle(UsersTakeQuery request, CancellationToken cancellationToken)
        {
            var users = request.Predicate == null
                ? await _userRepository.GetAllAsync(request.StartIndex, request.Count, cancellationToken)
                : await _userRepository.GetAllAsync(request.StartIndex, request.Count, request.Predicate, cancellationToken);

            if (users.IsFailure) return Result.Failure<List<UserResponse>>(users.Error);

            var listRes = users.Value.Select(u => new UserResponse(u)).ToList();

            return listRes;
        }
    }
}
