using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Users.Queries.GetAll
{
    internal sealed class UsersGetAllQueryHandler(IUserRepository userRepository) : IQueryHandler<UsersGetAllQuery, List<UserResponse>>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<List<UserResponse>>> Handle(UsersGetAllQuery request, CancellationToken cancellationToken)
        {
            var users = request.StartIndex == null || request.Count == null
                ? await _userRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
                : await _userRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
            if (users.IsFailure) return Result.Failure<List<UserResponse>>(users.Error);

            var listRes = users.Value.Select(u => new UserResponse(u)).ToList();

            return listRes;
        }
    }
}