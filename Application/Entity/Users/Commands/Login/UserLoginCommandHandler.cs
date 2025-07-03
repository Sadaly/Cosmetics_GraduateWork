using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Entity.Users.Commands.Login;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Users.Commands.UserLogin
{
    internal sealed class UserLoginCommandHandler : ICommandHandler<UserLoginCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;

        public UserLoginCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<Result<string>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            var passwordHash = PasswordHashed.Create(request.Password);

            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user.IsFailure) return Result.Failure<string>(
                PersistenceErrors.User.IncorrectUsernameOrPassword);

            var userpw = user.Value.PasswordHashed;
            if (userpw.Value != passwordHash.Value.Value) return Result.Failure<string>(
                PersistenceErrors.User.IncorrectUsernameOrPassword);

            string token = _jwtProvider.Generate(user.Value);

            return token;
        }
    }
}
