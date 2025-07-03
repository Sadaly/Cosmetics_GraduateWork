using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Users.Commands.SoftDelete
{
    public sealed class UserSoftDeleteCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : ICommandHandler<UserSoftDeleteCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(UserSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
            var update = await userRepository.RemoveAsync(user, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(update, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
