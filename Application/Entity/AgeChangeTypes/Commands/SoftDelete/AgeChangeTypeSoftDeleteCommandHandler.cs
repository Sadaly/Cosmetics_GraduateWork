using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.AgeChangeTypes.Commands.SoftDelete
{
	internal class AgeChangeTypeSoftDeleteCommandHandler(IAgeChangeTypeRepository ageChangeTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<AgeChangeTypeSoftDeleteCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(AgeChangeTypeSoftDeleteCommand request, CancellationToken cancellationToken)
		{
			var get = await ageChangeTypeRepository.GetByIdAsync(request.Id, cancellationToken);
			var remove = await ageChangeTypeRepository.RemoveAsync(get, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
