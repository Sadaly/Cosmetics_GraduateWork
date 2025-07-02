using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Repositories;

namespace Application.Entity
{
    internal sealed class TypeEntityDeletedDomainEventHandler<TypeE, TransitiveE>
        : IDomainEventHandler<EntityTypeDeletedDomainEvent>
        where TypeE : TypeEntity
        where TransitiveE : TransitiveEntity<TypeE>
    {
        private readonly IEntityTypeRepository<TypeE, TransitiveE> _entityTypeRepository;

        public TypeEntityDeletedDomainEventHandler(IEntityTypeRepository<TypeE, TransitiveE> entityTypeRepository)
        {
            _entityTypeRepository = entityTypeRepository;
        }

        public async Task Handle(EntityTypeDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            var entity = await _entityTypeRepository.GetByIdAsync(notification.EntityTypeId, cancellationToken);
            await _entityTypeRepository.RemoveAsync(entity);
        }

    }
}
