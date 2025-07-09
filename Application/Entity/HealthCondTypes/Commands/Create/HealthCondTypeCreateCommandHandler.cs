using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Entity.HealthCondTypes.Commands.Create
{
    internal class HealthCondTypeCreateCommandHandler(IHealthCondTypeRepository healthCondTypeRepository, IUnitOfWork unitOfWork) : ICommandHandler<HealthCondTypeCreateCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(HealthCondTypeCreateCommand request, CancellationToken cancellationToken)
        {
            var title = Title.Create(request.Title);
            var healthCondType = HealthCondType.Create(title);

            var add = await healthCondTypeRepository.AddAsync(healthCondType, cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(add, cancellationToken);

            return save.IsSuccess
                ? save.Value.Id
                : Result.Failure<Guid>(save.Error);
        }
    }
}
