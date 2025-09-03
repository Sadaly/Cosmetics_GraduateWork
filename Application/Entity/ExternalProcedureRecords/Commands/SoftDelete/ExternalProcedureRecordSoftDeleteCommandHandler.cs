using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ExternalProcedureRecords.Commands.SoftDelete
{
	internal class ExternalProcedureRecordSoftDeleteCommandHandler(IExternalProcedureRecordRepository externalProcedureRecordRepository, IUnitOfWork unitOfWork) : ICommandHandler<ExternalProcedureRecordSoftDeleteCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(ExternalProcedureRecordSoftDeleteCommand request, CancellationToken cancellationToken)
		{
			var get = await externalProcedureRecordRepository.GetByIdAsync(request.Id, cancellationToken);
			var remove = await externalProcedureRecordRepository.RemoveAsync(get, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
