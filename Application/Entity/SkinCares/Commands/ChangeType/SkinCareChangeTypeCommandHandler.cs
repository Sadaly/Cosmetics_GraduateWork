using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinCares.Commands.ChangeType
{
    internal class SkinCareChangeTypeCommandHandler(ISkinCareRepository skinCareRepository, ISkinCareTypeRepository skinCareTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinCareChangeTypeCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(SkinCareChangeTypeCommand request, CancellationToken cancellationToken)
        {
            var type = await skinCareTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);

            var ent = await skinCareRepository.GetByIdAsync(request.SkinCareId, cancellationToken);
            if (ent.IsFailure) return Result.Failure<Guid>(ent.Error);

            var update = ent.Value.ChangeType(type);
            if (update.IsFailure) return Result.Failure<Guid>(update.Error);

            var add = await skinCareRepository.UpdateAsync((SkinCare)update.Value, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
