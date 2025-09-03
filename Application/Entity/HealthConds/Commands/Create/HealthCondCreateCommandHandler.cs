using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.HealthConds.Commands.Create
{
	internal class HealthCondCreateCommandHandler(IHealthCondRepository healthCondRepository, IHealthCondTypeRepository healthCondTypeRepository, IPatientCardRepository patientCardRepository, IUnitOfWork unitOfWork) : ICommandHandler<HealthCondCreateCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(HealthCondCreateCommand request, CancellationToken cancellationToken)
		{
			var acType = await healthCondTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);
			var pc = await patientCardRepository.GetByIdAsync(request.PatientCardId, cancellationToken);
			var create = HealthCond.Create(pc, acType);

			var add = await healthCondRepository.AddAsync(create, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
