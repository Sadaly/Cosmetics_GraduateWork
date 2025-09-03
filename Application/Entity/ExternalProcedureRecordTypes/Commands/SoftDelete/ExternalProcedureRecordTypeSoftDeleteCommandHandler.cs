using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ExternalProcedureRecordTypes.Commands.SoftDelete
{
	internal class ExternalProcedureRecordTypeSoftDeleteCommandHandler(IExternalProcedureRecordTypeRepository externalProcedureRecordTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<ExternalProcedureRecordTypeSoftDeleteCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(ExternalProcedureRecordTypeSoftDeleteCommand request, CancellationToken cancellationToken)
		{
			var get = await externalProcedureRecordTypeRepository.GetByIdAsync(request.Id, cancellationToken);
			var remove = await externalProcedureRecordTypeRepository.RemoveAsync(get, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
