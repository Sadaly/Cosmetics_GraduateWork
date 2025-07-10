using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Procedures.Commands.ChangeType
{
    internal class ProcedureChangeTypeCommandHandler(IProcedureRepository procedureRepository, IProcedureTypeRepository procedureTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<ProcedureChangeTypeCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(ProcedureChangeTypeCommand request, CancellationToken cancellationToken)
        {
            var type = await procedureTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);

            var ent = await procedureRepository.GetByIdAsync(request.ProcedureId, cancellationToken);
            if (ent.IsFailure) return Result.Failure<Guid>(ent.Error);

            var update = ent.Value.ChangeType(type);
            if (update.IsFailure) return Result.Failure<Guid>(update.Error);

            var add = await procedureRepository.UpdateAsync((Procedure)update.Value, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
