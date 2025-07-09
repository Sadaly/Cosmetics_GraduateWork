using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinCares.Commands.SoftDelete
{
    internal class SkinCareSoftDeleteCommandHandler(ISkinCareRepository skinCareRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinCareSoftDeleteCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(SkinCareSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var get = await skinCareRepository.GetByIdAsync(request.Id, cancellationToken);
            var remove = await skinCareRepository.RemoveAsync(get, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
