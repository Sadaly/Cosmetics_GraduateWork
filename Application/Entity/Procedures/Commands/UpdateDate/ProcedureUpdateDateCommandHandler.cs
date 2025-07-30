using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Procedures.Commands.UpdateDate
{
    internal class ProcedureUpdateDateCommandHandler(IProcedureScheduleService procedureScheduleService, IProcedureRepository procedureRepository, IUnitOfWork unitOfWork) : ICommandHandler<ProcedureUpdateDateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(ProcedureUpdateDateCommand request, CancellationToken cancellationToken)
        {
            var isDateReserved = await procedureScheduleService.IsDateReserved(request.ScheduledDate, request.ScheduledDate.AddMinutes(request.Duration), cancellationToken);
            if (isDateReserved) return Result.Failure<Guid>(ApplicationErrors.ProcedureCreateCommand.DateReserved);

            var ent = await procedureRepository.GetByIdAsync(request.ProcedureId, cancellationToken);
            if (ent.IsFailure) return Result.Failure<Guid>(ent.Error);

            var update = ent.Value.UpdateDate(request.ScheduledDate, request.Duration);
            var add = await procedureRepository.UpdateAsync(update, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
