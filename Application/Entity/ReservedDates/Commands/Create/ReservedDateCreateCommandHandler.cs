using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.ReservedDates.Commands.Create
{
	internal class ReservedDateCreateCommandHandler(IReservedDateRepository reservedDateRepository, IUnitOfWork unitOfWork) : ICommandHandler<ReservedDateCreateCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(ReservedDateCreateCommand request, CancellationToken cancellationToken)
		{
			var create = ReservedDate.Create(request.StartDate, request.EndDate, request.Type);
			var add = await reservedDateRepository.AddAsync(create, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
