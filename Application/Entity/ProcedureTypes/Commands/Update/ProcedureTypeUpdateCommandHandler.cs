using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.ProcedureTypes.Commands.Update
{
    internal class ProcedureTypeUpdateCommandHandler(IProcedureTypeRepository procedureTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<ProcedureTypeUpdateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(ProcedureTypeUpdateCommand request, CancellationToken cancellationToken)
        {
            var procedureType = await procedureTypeRepository.GetByIdAsync(request.ProcedureTypeId, cancellationToken);
            if (procedureType.IsFailure) return Result.Failure<Guid>(procedureType.Error);

            if (request.Title != null)
            {
                var update = procedureType.Value.Update(Title.Create(request.Title));
                if (update.IsFailure) return Result.Failure<Guid>(update.Error);
            }

            if (request.Descr != null) {
                var update = procedureType.Value.UpdateDescription(request.Descr);
                if (update.IsFailure) return Result.Failure<Guid>(update.Error);
            }

            if (request.Duration != null)
            {
                var update = procedureType.Value.UpdateStandartDuration(request.Duration);
                if (update.IsFailure) return Result.Failure<Guid>(update.Error);
            }

            var add = await procedureTypeRepository.UpdateAsync(procedureType.Value, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
