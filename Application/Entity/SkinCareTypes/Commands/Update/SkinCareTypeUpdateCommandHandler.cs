using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.SkinCareTypes.Commands.Update
{
	internal class SkinCareTypeUpdateCommandHandler(ISkinCareTypeRepository skinCareTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinCareTypeUpdateCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(SkinCareTypeUpdateCommand request, CancellationToken cancellationToken)
		{
			var title = Title.Create(request.Title);

			var skinCareType = await skinCareTypeRepository.GetByIdAsync(request.SkinCareTypeId, cancellationToken);
			if (skinCareType.IsFailure) return Result.Failure<Guid>(skinCareType.Error);

			var update = skinCareType.Value.Update(title);
			if (update.IsFailure) return Result.Failure<Guid>(update.Error);

			var add = await skinCareTypeRepository.UpdateAsync((SkinCareType)update.Value, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
