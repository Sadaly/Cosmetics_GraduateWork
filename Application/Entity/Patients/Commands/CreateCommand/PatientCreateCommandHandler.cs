using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Patients.Commands.PatientCreateCommand
{
    internal class PatientCreateCommandHandler : ICommandHandler<PatientCreateCommand, Guid>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientCardRepository _patientCardRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PatientCreateCommandHandler(IPatientRepository patientRepository, IUnitOfWork unitOfWork, IPatientCardRepository patientCardRepository)
        {
            _patientRepository = patientRepository;
            _unitOfWork = unitOfWork;
            _patientCardRepository = patientCardRepository;
        }

        public async Task<Result<Guid>> Handle(PatientCreateCommand request, CancellationToken cancellationToken)
        {
            var fullname = Username.Create(request.FullName);
            var patient = Patient.Create(fullname);

            var addp = await _patientRepository.AddAsync(patient, cancellationToken);
            if (addp.IsFailure) return Result.Failure<Guid>(addp.Error);

            var addpc = await _patientCardRepository.AddAsync(patient.Value.Card, cancellationToken);
            var save = await _unitOfWork.SaveChangesAsync(addpc, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
