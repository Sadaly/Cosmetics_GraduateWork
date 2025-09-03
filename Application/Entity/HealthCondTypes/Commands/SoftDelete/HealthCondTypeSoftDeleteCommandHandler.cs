using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.HealthCondTypes.Commands.SoftDelete
{
	internal class HealthCondTypeSoftDeleteCommandHandler(IHealthCondTypeRepository healthCondTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<HealthCondTypeSoftDeleteCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(HealthCondTypeSoftDeleteCommand request, CancellationToken cancellationToken)
		{
			var get = await healthCondTypeRepository.GetByIdAsync(request.Id, cancellationToken);
			var remove = await healthCondTypeRepository.RemoveAsync(get, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
