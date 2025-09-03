using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinFeatures.Commands.ChangeType
{
	internal class SkinFeatureChangeTypeCommandHandler(ISkinFeatureRepository skinFeatureRepository, ISkinFeatureTypeRepository skinFeatureTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinFeatureChangeTypeCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(SkinFeatureChangeTypeCommand request, CancellationToken cancellationToken)
		{
			var type = await skinFeatureTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);

			var ent = await skinFeatureRepository.GetByIdAsync(request.SkinFeatureId, cancellationToken);
			if (ent.IsFailure) return Result.Failure<Guid>(ent.Error);

			var update = ent.Value.ChangeType(type);
			if (update.IsFailure) return Result.Failure<Guid>(update.Error);

			var add = await skinFeatureRepository.UpdateAsync((SkinFeature)update.Value, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
