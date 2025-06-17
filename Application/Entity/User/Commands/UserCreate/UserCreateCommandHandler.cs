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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public UserCreateCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task<Result<Guid>> Handle(UserCreateCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            var username = Username.Create(request.Username);
            var password = PasswordHashed.Create(request.Password);

            var user = User.Create(email, username, password);
            if (user.IsFailure) return Result.Failure<Guid>(user.Error);

            var add = await _userRepository.AddAsync(user.Value, cancellationToken);
            if (add.IsFailure) return Result.Failure<Guid>(add.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return user.Value.Id;
        }
    }
}
