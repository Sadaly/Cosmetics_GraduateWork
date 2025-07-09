using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.AgeChanges.Commands.ChangeType
{
    internal class AgeChangeChangeTypeCommandHandler(IAgeChangeRepository ageChangeRepository, IAgeChangeTypeRepository ageChangeTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<AgeChangeChangeTypeCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AgeChangeChangeTypeCommand request, CancellationToken cancellationToken)
        {
            var acType = await ageChangeTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);
            var ent = await ageChangeRepository.GetByIdAsync(request.AgeChangeId, cancellationToken);
            if (ent.IsFailure) return Result.Failure<Guid>(ent.Error);
            var update = ent.Value.ChangeType(acType);
            if (update.IsFailure) return Result.Failure<Guid>(update.Error);
            var after = (AgeChange)update.Value;
            var add = await ageChangeRepository.UpdateAsync(after, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
