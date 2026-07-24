using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Procedures.Commands.Proceed
{
	internal class ProcedureProceedCommandHandler(IProcedureRepository procedureRepository, IUnitOfWork unitOfWork) : ICommandHandler<ProcedureProceedCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(ProcedureProceedCommand request, CancellationToken cancellationToken)
		{
			var ent = await procedureRepository.GetByIdAsync(request.ProcedureId, cancellationToken);
			if (ent.IsFailure) return Result.Failure<Guid>(ent.Error);

			var update = ent.Value.Proceed();

			var add = await procedureRepository.UpdateAsync(update, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
