using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Users.Commands.UserCreate
{
    public sealed class UserUpdateCommandHandler : ICommandHandler<UserUpdateCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserUpdateCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UserUpdateCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user.IsFailure) return Result.Failure<Guid>(user.Error);

            var email = Email.CreateOrDefault(request.Email);
            var username = Username.CreateOrDefault(request.Username);
            var password = PasswordHashed.CreateOrDefault(request.Password);

            user.Value.UpdateEmail(email)
                .Value.UpdateUsername(username)
                .Value.UpdatePassword(password);

            var add = await _userRepository.AddAsync(user, cancellationToken);
            var save = await _unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
