using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Patients.Commands.Create
{
    internal class PatientCreateCommandHandler(IPatientRepository patientRepository, IUnitOfWork unitOfWork, IPatientCardRepository patientCardRepository) : ICommandHandler<PatientCreateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(PatientCreateCommand request, CancellationToken cancellationToken)
        {
            var fullname = Username.Create(request.FullName);
            var patient = Patient.Create(fullname);

            var addp = await patientRepository.AddAsync(patient, cancellationToken);
            if (addp.IsFailure) return Result.Failure<Guid>(addp.Error);

            var addpc = await patientCardRepository.AddAsync(patient.Value.Card, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(addpc, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
