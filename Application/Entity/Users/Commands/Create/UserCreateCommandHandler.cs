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
        private readonly IUnitOfWork _unitOfWork;

        public UserCreateCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UserCreateCommand request, CancellationToken cancellationToken)
        {
            var email    = Email.Create(request.Email);
            var username = Username.Create(request.Username);
            var password = PasswordHashed.Create(request.Password);

            var user = User.Create(email, username, password);
            var add = await _userRepository.AddAsync(user, cancellationToken);
            var save = await _unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess 
                ? save.Value.Id 
                : Result.Failure<Guid>(save.Error);
        }
    }
}
