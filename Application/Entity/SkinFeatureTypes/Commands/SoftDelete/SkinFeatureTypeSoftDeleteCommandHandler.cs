using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.SkinFeatureTypes.Commands.SoftDelete
{
    internal class SkinFeatureTypeSoftDeleteCommandHandler(ISkinFeatureTypeRepository skinFeatureTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<SkinFeatureTypeSoftDeleteCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(SkinFeatureTypeSoftDeleteCommand request, CancellationToken cancellationToken)
        {
            var get = await skinFeatureTypeRepository.GetByIdAsync(request.Id, cancellationToken);
            var remove = await skinFeatureTypeRepository.RemoveAsync(get, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(remove, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
