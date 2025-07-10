using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.PatientSpecificses.Commands.Create
{
    internal class PatientSpecificsCreateCommandHandler(IPatientSpecificsRepository patientSpecificsRepository, IPatientCardRepository patientCardRepository, IUnitOfWork unitOfWork) : ICommandHandler<PatientSpecificsCreateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(PatientSpecificsCreateCommand request, CancellationToken cancellationToken)
        {
            var pc = await patientCardRepository.GetByIdAsync(request.PatientCardId, cancellationToken);
            var create = PatientSpecifics.Create(request.Sleep, request.Diet, request.Sport, request.WorkEnviroment, pc);

            var add = await patientSpecificsRepository.AddAsync(create, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
