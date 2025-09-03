using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.HealthConds.Commands.SoftDelete
{
	internal class HealthCondSoftDeleteCommandHandler(IHealthCondRepository healthCondRepository, IUnitOfWork unitOfWork) : ICommandHandler<HealthCondSoftDeleteCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(HealthCondSoftDeleteCommand request, CancellationToken cancellationToken)
		{
			var get = await healthCondRepository.GetByIdAsync(request.Id, cancellationToken);
			var remove = await healthCondRepository.RemoveAsync(get, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
