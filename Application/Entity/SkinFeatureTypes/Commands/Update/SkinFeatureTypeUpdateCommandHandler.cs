using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.SkinFeatureTypes.Commands.Update
{
	internal class SkinFeatureTypeUpdateCommandHandler(ISkinFeatureTypeRepository skinFeatureTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinFeatureTypeUpdateCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(SkinFeatureTypeUpdateCommand request, CancellationToken cancellationToken)
		{
			var title = Title.Create(request.Title);

			var skinFeatureType = await skinFeatureTypeRepository.GetByIdAsync(request.SkinFeatureTypeId, cancellationToken);
			if (skinFeatureType.IsFailure) return Result.Failure<Guid>(skinFeatureType.Error);

			var update = skinFeatureType.Value.Update(title);
			if (update.IsFailure) return Result.Failure<Guid>(update.Error);

			var add = await skinFeatureTypeRepository.UpdateAsync((SkinFeatureType)update.Value, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
