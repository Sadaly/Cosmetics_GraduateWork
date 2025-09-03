using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinCareTypes.Commands.SoftDelete
{
	internal class SkinCareTypeSoftDeleteCommandHandler(ISkinCareTypeRepository skinCareTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinCareTypeSoftDeleteCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(SkinCareTypeSoftDeleteCommand request, CancellationToken cancellationToken)
		{
			var get = await skinCareTypeRepository.GetByIdAsync(request.Id, cancellationToken);
			var remove = await skinCareTypeRepository.RemoveAsync(get, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
