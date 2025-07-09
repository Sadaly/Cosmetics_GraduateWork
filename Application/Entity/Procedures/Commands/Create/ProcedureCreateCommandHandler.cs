using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Procedures.Commands.Create
{
    internal class ProcedureCreateCommandHandler(IProcedureRepository procedureRepository, IProcedureTypeRepository procedureTypeRepository, IDoctorRepository doctorRepository, IPatientCardRepository patientCardRepository, IUnitOfWork unitOfWork) : ICommandHandler<ProcedureCreateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(ProcedureCreateCommand request, CancellationToken cancellationToken)
        {
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
