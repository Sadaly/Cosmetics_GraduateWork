using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Patients.Commands.SoftDelete
{
    internal class PatientSoftDeleteCommandHandler(IUnitOfWork unitOfWork, IPatientRepository patientRepository, IPatientCardRepository patientCardRepository) : ICommandHandler<PatientSoftDeleteCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(PatientSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var patient = await patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
            if (patient.IsFailure) return Result.Failure<Guid>(patient.Error);

            var deletePatient = await patientRepository.RemoveAsync(patient, cancellationToken);
            if (deletePatient.IsFailure) return Result.Failure<Guid>(deletePatient.Error);

            var deletePatientCard = await patientCardRepository.RemoveAsync(patient.Value.Card, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(deletePatientCard, cancellationToken);
            
            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
