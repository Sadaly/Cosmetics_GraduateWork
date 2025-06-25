using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Users.Commands.UserCreate
{
    public sealed class UserCreateCommandHandler : ICommandHandler<UserCreateCommand, Guid>
    {
        private readonly IUserRepository _userRepository;

        public UserCreateCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<Guid>> Handle(UserCreateCommand request, CancellationToken cancellationToken)
        {
            var email    = Email.Create(request.Email);
            var username = Username.Create(request.Username);
            var password = PasswordHashed.Create(request.Password);

            var user = User.Create(email, username, password);
            var add = await _userRepository.AddAsync(user, cancellationToken);

            return add.IsSuccess 
                ? add.Value.Id 
                : Result.Failure<Guid>(add.Error);
        }
    }
}
