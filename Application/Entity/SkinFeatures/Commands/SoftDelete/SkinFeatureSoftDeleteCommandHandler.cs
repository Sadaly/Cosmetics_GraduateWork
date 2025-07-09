using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinFeatures.Commands.SoftDelete
{
    internal class SkinFeatureSoftDeleteCommandHandler(ISkinFeatureRepository skinFeatureRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinFeatureSoftDeleteCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(SkinFeatureSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var get = await skinFeatureRepository.GetByIdAsync(request.Id, cancellationToken);
            var remove = await skinFeatureRepository.RemoveAsync(get, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
