using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ProcedureTypes.Commands.SoftDelete
{
    internal class ProcedureTypeSoftDeleteCommandHandler(IProcedureTypeRepository procedureTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<ProcedureTypeSoftDeleteCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(ProcedureTypeSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var get = await procedureTypeRepository.GetByIdAsync(request.Id, cancellationToken);
            var remove = await procedureTypeRepository.RemoveAsync(get, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
