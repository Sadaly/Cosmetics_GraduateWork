using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.PatientSpecificses.Commands.SoftDelete
{
    internal class PatientSpecificsSoftDeleteCommandHandler(IPatientSpecificsRepository patientSpecificsRepository, IUnitOfWork unitOfWork) : ICommandHandler<PatientSpecificsSoftDeleteCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(PatientSpecificsSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var get = await patientSpecificsRepository.GetByIdAsync(request.Id, cancellationToken);
            var remove = await patientSpecificsRepository.RemoveAsync(get, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
