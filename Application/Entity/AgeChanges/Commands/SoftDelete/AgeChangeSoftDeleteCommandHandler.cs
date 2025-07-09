using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.AgeChanges.Commands.SoftDelete
{
    internal class AgeChangeSoftDeleteCommandHandler(IAgeChangeRepository ageChangeRepository, IUnitOfWork unitOfWork) : ICommandHandler<AgeChangeSoftDeleteCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AgeChangeSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var get = await ageChangeRepository.GetByIdAsync(request.Id, cancellationToken);
            var remove = await ageChangeRepository.RemoveAsync(get, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
