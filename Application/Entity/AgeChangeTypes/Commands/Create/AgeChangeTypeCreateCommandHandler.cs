using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.AgeChangeTypes.Commands.Create
{
	internal class AgeChangeTypeCreateCommandHandler(IAgeChangeTypeRepository ageChangeTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<AgeChangeTypeCreateCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(AgeChangeTypeCreateCommand request, CancellationToken cancellationToken)
		{
			var title = Title.Create(request.Title);
			var ageChangeType = AgeChangeType.Create(title);

			var add = await ageChangeTypeRepository.AddAsync(ageChangeType, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
