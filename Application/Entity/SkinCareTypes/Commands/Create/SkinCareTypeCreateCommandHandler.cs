using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.SkinCareTypes.Commands.Create
{
	internal class SkinCareTypeCreateCommandHandler(ISkinCareTypeRepository skinCareTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinCareTypeCreateCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(SkinCareTypeCreateCommand request, CancellationToken cancellationToken)
		{
			var title = Title.Create(request.Title);
			var skinCareType = SkinCareType.Create(title);

			var add = await skinCareTypeRepository.AddAsync(skinCareType, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
