using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.AgeChangeTypes.Commands.Update
{
	internal class AgeChangeTypeUpdateCommandHandler(IAgeChangeTypeRepository ageChangeTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<AgeChangeTypeUpdateCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(AgeChangeTypeUpdateCommand request, CancellationToken cancellationToken)
		{
			var title = Title.Create(request.Title);

			var ageChangeType = await ageChangeTypeRepository.GetByIdAsync(request.AgeChangeTypeId, cancellationToken);
			if (ageChangeType.IsFailure) return Result.Failure<Guid>(ageChangeType.Error);

			var update = ageChangeType.Value.Update(title);
			if (update.IsFailure) return Result.Failure<Guid>(update.Error);

			var add = await ageChangeTypeRepository.UpdateAsync((AgeChangeType)update.Value, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
