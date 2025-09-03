using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinFeatures.Commands.Create
{
	internal class SkinFeatureCreateCommandHandler(ISkinFeatureRepository skinFeatureRepository, ISkinFeatureTypeRepository skinFeatureTypeRepository, IPatientCardRepository patientCardRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinFeatureCreateCommand, Guid>
	{
		public async Task<Result<Guid>> Handle(SkinFeatureCreateCommand request, CancellationToken cancellationToken)
		{
			var acType = await skinFeatureTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);
			var pc = await patientCardRepository.GetByIdAsync(request.PatientCardId, cancellationToken);
			var create = SkinFeature.Create(pc, acType);

			var add = await skinFeatureRepository.AddAsync(create, cancellationToken);
			var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

			return save.IsSuccess
				? save.Value.Id
				: Result.Failure<Guid>(save.Error);
		}
	}
}
