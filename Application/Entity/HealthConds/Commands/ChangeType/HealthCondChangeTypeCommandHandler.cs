using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.HealthConds.Commands.ChangeType
{
    internal class HealthCondChangeTypeCommandHandler(IHealthCondRepository healthCondRepository, IHealthCondTypeRepository healthCondTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<HealthCondChangeTypeCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(HealthCondChangeTypeCommand request, CancellationToken cancellationToken)
        {
            var type = await healthCondTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);

            var ent = await healthCondRepository.GetByIdAsync(request.HealthCondId, cancellationToken);
            if (ent.IsFailure) return Result.Failure<Guid>(ent.Error);

            var update = ent.Value.ChangeType(type);
            if (update.IsFailure) return Result.Failure<Guid>(update.Error);

            var add = await healthCondRepository.UpdateAsync((HealthCond)update.Value, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
