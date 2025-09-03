using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.ExternalProcedureRecordTypes.Commands.Create
{
	internal class ExternalProcedureRecordTypeCreateCommandHandler(IExternalProcedureRecordTypeRepository externalProcedureRecordTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<ExternalProcedureRecordTypeCreateCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(ExternalProcedureRecordTypeCreateCommand request, CancellationToken cancellationToken)
		{
			var title = Title.Create(request.Title);
			var externalProcedureRecordType = ExternalProcedureRecordType.Create(title);

			var add = await externalProcedureRecordTypeRepository.AddAsync(externalProcedureRecordType, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
