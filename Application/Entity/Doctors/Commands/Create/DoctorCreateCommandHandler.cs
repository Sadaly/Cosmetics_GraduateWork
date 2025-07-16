using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.Doctors.Commands.Create
{
    internal class DoctorCreateCommandHandler(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork) : ICommandHandler<DoctorCreateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(DoctorCreateCommand request, CancellationToken cancellationToken)
        {
            var name = Username.Create(request.Name);
            var doctor = Doctor.Create(name);

            var add = await doctorRepository.AddAsync(doctor, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
