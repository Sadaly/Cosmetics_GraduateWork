using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ReservedDates.Commands.SoftDelete
{
	internal class ReservedDateSoftDeleteCommandHandler(IReservedDateRepository reservedDateRepository, IUnitOfWork unitOfWork) : ICommandHandler<ReservedDateSoftDeleteCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(ReservedDateSoftDeleteCommand request, CancellationToken cancellationToken)
		{
			var get = await reservedDateRepository.GetByIdAsync(request.Id, cancellationToken);
			var remove = await reservedDateRepository.RemoveAsync(get, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
