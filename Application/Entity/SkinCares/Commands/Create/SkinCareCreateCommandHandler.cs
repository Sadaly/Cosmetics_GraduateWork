using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinCares.Commands.Create
{
	internal class SkinCareCreateCommandHandler(ISkinCareRepository skinCareRepository, ISkinCareTypeRepository skinCareTypeRepository, IPatientCardRepository patientCardRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinCareCreateCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(SkinCareCreateCommand request, CancellationToken cancellationToken)
		{
			var acType = await skinCareTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);
			var pc = await patientCardRepository.GetByIdAsync(request.PatientCardId, cancellationToken);
			var create = SkinCare.Create(pc, acType);

			var add = await skinCareRepository.AddAsync(create, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
