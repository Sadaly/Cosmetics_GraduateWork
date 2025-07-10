using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Procedures.Commands.Create
{
    internal class ProcedureCreateCommandHandler(IReservedDateRepository reservedDateRepository, IProcedureRepository procedureRepository, IProcedureTypeRepository procedureTypeRepository, IDoctorRepository doctorRepository, IPatientCardRepository patientCardRepository, IUnitOfWork unitOfWork) : ICommandHandler<ProcedureCreateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(ProcedureCreateCommand request, CancellationToken cancellationToken)
        {
            if (request.ScheduledDate != null)
            { 
                var rd = await reservedDateRepository.GetByPredicateAsync(x => 
                    (x.StartDate < request.ScheduledDate.Value.AddMinutes(request.Duration) && x.EndDate >= request.ScheduledDate.Value.AddMinutes(request.Duration))
                    || (x.StartDate >= request.ScheduledDate && x.EndDate < request.ScheduledDate)
                    || (x.StartDate > request.ScheduledDate && x.EndDate < request.ScheduledDate.Value.AddMinutes(request.Duration)),
                    cancellationToken);

                if (rd.IsFailure) return Result.Failure<Guid>(ApplicationErrors.ProcedureCreateCommand.DateReserved);
            }
            var acType = await procedureTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);
            var pc = await patientCardRepository.GetByIdAsync(request.PatientCardId, cancellationToken);
            var doctor = request.DoctorId != null ?
                await doctorRepository.GetByIdAsync(request.DoctorId.Value, cancellationToken)
                : null;
            var create = Procedure.Create(pc, acType, request.Duration, request.ScheduledDate, doctor?.Value);

            var add = await procedureRepository.AddAsync(create, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
