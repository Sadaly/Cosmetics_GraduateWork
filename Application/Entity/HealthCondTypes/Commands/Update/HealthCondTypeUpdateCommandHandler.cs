using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.HealthCondTypes.Commands.Update
{
    internal class HealthCondTypeUpdateCommandHandler(IHealthCondTypeRepository healthCondTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<HealthCondTypeUpdateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(HealthCondTypeUpdateCommand request, CancellationToken cancellationToken)
        {
            var title = Title.Create(request.Title);

            var healthCondType = await healthCondTypeRepository.GetByIdAsync(request.HealthCondTypeId, cancellationToken);
            if (healthCondType.IsFailure) return Result.Failure<Guid>(healthCondType.Error);

            var update = healthCondType.Value.Update(title);
            if (update.IsFailure) return Result.Failure<Guid>(update.Error);

            var add = await healthCondTypeRepository.UpdateAsync((HealthCondType)update.Value, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
