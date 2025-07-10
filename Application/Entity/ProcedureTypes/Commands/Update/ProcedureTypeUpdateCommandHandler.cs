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
            var title = Title.Create(request.Title);

            var procedureType = await procedureTypeRepository.GetByIdAsync(request.ProcedureTypeId, cancellationToken);
            if (procedureType.IsFailure) return Result.Failure<Guid>(procedureType.Error);

            var update = procedureType.Value.Update(title);
            if (update.IsFailure) return Result.Failure<Guid>(update.Error);

            var add = await procedureTypeRepository.UpdateAsync((ProcedureType)update.Value, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
