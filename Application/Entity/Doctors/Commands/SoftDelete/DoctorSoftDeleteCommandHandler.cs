using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Doctors.Commands.SoftDelete
{
    internal class DoctorSoftDeleteCommandHandler(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork) : ICommandHandler<DoctorSoftDeleteCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(DoctorSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var get = await doctorRepository.GetByIdAsync(request.Id, cancellationToken);
            var remove = await doctorRepository.RemoveAsync(get, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
