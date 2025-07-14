using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Procedures.Commands.SoftDelete
{
    internal class ProcedureSoftDeleteCommandHandler(IProcedureRepository procedureRepository, IUnitOfWork unitOfWork) : ICommandHandler<ProcedureSoftDeleteCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(ProcedureSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var get = await procedureRepository.GetByIdAsync(request.Id, cancellationToken, FetchMode.Include);
            var remove = await procedureRepository.RemoveAsync(get, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
