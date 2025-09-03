using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ExternalProcedureRecords.Commands.ChangeType
{
	internal class ExternalProcedureRecordChangeTypeCommandHandler(IExternalProcedureRecordRepository externalProcedureRecordRepository, IExternalProcedureRecordTypeRepository externalProcedureRecordTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<ExternalProcedureRecordChangeTypeCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(ExternalProcedureRecordChangeTypeCommand request, CancellationToken cancellationToken)
		{
			var type = await externalProcedureRecordTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);

			var ent = await externalProcedureRecordRepository.GetByIdAsync(request.ExternalProcedureRecordId, cancellationToken);
			if (ent.IsFailure) return Result.Failure<Guid>(ent.Error);

			var update = ent.Value.ChangeType(type);
			if (update.IsFailure) return Result.Failure<Guid>(update.Error);

			var add = await externalProcedureRecordRepository.UpdateAsync((ExternalProcedureRecord)update.Value, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
