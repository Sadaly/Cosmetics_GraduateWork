using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Patients.Commands.SoftDelete
{
    internal class PatientSoftDeleteCommandHandler(IPatientRepository patientRepository, IUnitOfWork unitOfWork) : ICommandHandler<PatientSoftDeleteCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(PatientSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var patient = await patientRepository.GetByIdAsync(request.PatientId, cancellationToken, FetchMode.Include);
            if (patient.IsFailure) return Result.Failure<Guid>(patient.Error);

            var deletePatient = await patientRepository.RemoveAsync(patient, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(deletePatient, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
