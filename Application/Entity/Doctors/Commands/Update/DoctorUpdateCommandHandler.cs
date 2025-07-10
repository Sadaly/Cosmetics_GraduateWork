using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Doctors.Commands.Update
{
    internal class DoctorUpdateCommandHandler(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork) : ICommandHandler<DoctorUpdateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(DoctorUpdateCommand request, CancellationToken cancellationToken)
        {
            var name = Username.Create(request.Name);
            var doctor = await doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
            if (doctor.IsFailure) return Result.Failure<Guid>(doctor.Error);

            var update = doctor.Value.Update(name);
            var add = await doctorRepository.UpdateAsync(update, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
