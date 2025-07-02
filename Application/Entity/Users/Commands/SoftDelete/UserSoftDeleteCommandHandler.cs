using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Users.Commands.UserCreate
{
    public sealed class UserSoftDeleteCommandHandler : ICommandHandler<UserSoftDeleteCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserSoftDeleteCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UserSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            var update = await _userRepository.RemoveAsync(user, cancellationToken);
            var save = await _unitOfWork.SaveChangesAsync(update, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
