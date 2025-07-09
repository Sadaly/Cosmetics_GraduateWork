using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ExternalProcedureRecords.Commands.Create
{
    internal class ExternalProcedureRecordCreateCommandHandler(IExternalProcedureRecordRepository externalProcedureRecordRepository, IExternalProcedureRecordTypeRepository externalProcedureRecordTypeRepository, IPatientCardRepository patientCardRepository, IUnitOfWork unitOfWork) : ICommandHandler<ExternalProcedureRecordCreateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(ExternalProcedureRecordCreateCommand request, CancellationToken cancellationToken)
        {
            var acType = await externalProcedureRecordTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);
            var pc = await patientCardRepository.GetByIdAsync(request.PatientCardId, cancellationToken);
            var create = ExternalProcedureRecord.Create(pc, acType, request.Date);

            var add = await externalProcedureRecordRepository.AddAsync(create, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
