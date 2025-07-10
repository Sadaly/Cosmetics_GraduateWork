using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Patients.Commands.Update
{
    internal class PatientUpdateCommandHandler(IPatientRepository patientRepository, IUnitOfWork unitOfWork) : ICommandHandler<PatientUpdateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(PatientUpdateCommand request, CancellationToken cancellationToken)
        {
            var fullname = Username.Create(request.FullName);
            var patient = await patientRepository.GetByIdAsync(request.Id, cancellationToken);
            if (patient.IsFailure) return Result.Failure<Guid>(patient.Error);
            var update = await patientRepository.UpdateAsync(patient.Value.UpdateFullname(fullname), cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(update, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
