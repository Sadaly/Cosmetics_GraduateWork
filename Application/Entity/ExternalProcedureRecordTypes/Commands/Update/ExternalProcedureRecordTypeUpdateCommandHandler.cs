using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.ExternalProcedureRecordTypes.Commands.Update
{
    internal class ExternalProcedureRecordTypeUpdateCommandHandler(IExternalProcedureRecordTypeRepository externalProcedureRecordTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<ExternalProcedureRecordTypeUpdateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(ExternalProcedureRecordTypeUpdateCommand request, CancellationToken cancellationToken)
        {
            var title = Title.Create(request.Title);

            var externalProcedureRecordType = await externalProcedureRecordTypeRepository.GetByIdAsync(request.ExternalProcedureRecordTypeId, cancellationToken);
            if (externalProcedureRecordType.IsFailure) return Result.Failure<Guid>(externalProcedureRecordType.Error);

            var update = externalProcedureRecordType.Value.Update(title);
            if (update.IsFailure) return Result.Failure<Guid>(update.Error);

            var add = await externalProcedureRecordTypeRepository.UpdateAsync((ExternalProcedureRecordType)update.Value, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
