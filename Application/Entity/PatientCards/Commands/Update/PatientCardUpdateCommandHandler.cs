using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.PatientCards.Commands.Update
{
    internal class PatientCardUpdateCommandHandler(IPatientCardRepository patientcardRepository, IUnitOfWork unitOfWork) : ICommandHandler<PatientCardUpdateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(PatientCardUpdateCommand request, CancellationToken cancellationToken)
        {
            var patientcard = await patientcardRepository.GetByIdAsync(request.Id, cancellationToken);
            if (patientcard.IsFailure) return Result.Failure<Guid>(patientcard.Error);

            var address = Text.Create(request.Address);
            var complaints = Text.Create(request.Complaints);
            var phone = PhoneNumber.Create(request.PhoneNumber);
            var update = patientcard.Value.Update(request.Age, address, complaints, phone);

            var add = await patientcardRepository.UpdateAsync(update, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
