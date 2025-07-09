using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.ProcedureTypes.Commands.Create
{
    internal class ProcedureTypeCreateCommandHandler(IProcedureTypeRepository procedureTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<ProcedureTypeCreateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(ProcedureTypeCreateCommand request, CancellationToken cancellationToken)
        {
            var title = Title.Create(request.Title);
            var procedureType = ProcedureType.Create(title, request.Description, request.StandartDur);

            var add = await procedureTypeRepository.AddAsync(procedureType, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
