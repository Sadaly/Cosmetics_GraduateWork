using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinFeatures.Commands.Create
{
    internal class SkinFeatureCreateCommandHandler(ISkinFeatureTypeRepository skinFeatureTypeRepository, ISkinFeatureRepository skinFeatureRepository, IPatientCardRepository patientCardRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinFeatureCreateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(SkinFeatureCreateCommand request, CancellationToken cancellationToken)
        {
            var skinFeatureType = await skinFeatureTypeRepository.GetByIdAsync(request.TypeId, cancellationToken);
            var patientCard = await patientCardRepository.GetByIdAsync(request.PatientCardId, cancellationToken);
            var skinFeature = SkinFeature.Create(patientCard, skinFeatureType);

            var add = await skinFeatureRepository.AddAsync(skinFeature, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
