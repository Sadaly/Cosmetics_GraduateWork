using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Procedures.Commands.UpdateDate
{
    internal class ProcedureUpdateDateCommandHandler(IReservedDateRepository reservedDateRepository, IProcedureRepository procedureRepository, IUnitOfWork unitOfWork) : ICommandHandler<ProcedureUpdateDateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(ProcedureUpdateDateCommand request, CancellationToken cancellationToken)
        {
            var rd = await reservedDateRepository.GetByPredicateAsync(x =>
                (x.StartDate < request.ScheduledDate.AddMinutes(request.Duration) && x.EndDate >= request.ScheduledDate.AddMinutes(request.Duration))
                || (x.StartDate >= request.ScheduledDate && x.EndDate < request.ScheduledDate)
                || (x.StartDate > request.ScheduledDate && x.EndDate < request.ScheduledDate.AddMinutes(request.Duration)),
                cancellationToken);

            if (rd.IsFailure) return Result.Failure<Guid>(ApplicationErrors.ProcedureCreateCommand.DateReserved);
            
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
