using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.SkinFeatureTypes.Commands.Create
{
	internal class SkinFeatureTypeCreateCommandHandler(ISkinFeatureTypeRepository skinFeatureTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinFeatureTypeCreateCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(SkinFeatureTypeCreateCommand request, CancellationToken cancellationToken)
		{
			var title = Title.Create(request.Title);
			var skinFeatureType = SkinFeatureType.Create(title);

			var add = await skinFeatureTypeRepository.AddAsync(skinFeatureType, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
