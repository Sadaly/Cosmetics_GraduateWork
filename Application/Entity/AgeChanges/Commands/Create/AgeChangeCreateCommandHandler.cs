using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.AgeChanges.Commands.Create
{
    internal class AgeChangeCreateCommandHandler(IAgeChangeRepository ageChangeRepository, IAgeChangeTypeRepository ageChangeTypeRepository, IPatientCardRepository patientCardRepository, IUnitOfWork unitOfWork) : ICommandHandler<AgeChangeCreateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AgeChangeCreateCommand request, CancellationToken cancellationToken)
        {
            var acType = await ageChangeTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);
            var pc = await patientCardRepository.GetByIdAsync(request.PatientCardId, cancellationToken);
            var create = AgeChange.Create(pc, acType);

            var add = await ageChangeRepository.AddAsync(create, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
