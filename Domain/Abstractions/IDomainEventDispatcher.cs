using Domain.Common;

namespace Domain.Abstractions
{
    /// <summary>
    /// Этот интерфейс объявляет метод, который можно использовать для отправки событий предметной области по всему приложению.
    /// </summary>
    public interface IDomainEventDispatcher
    {
        Task DispatchAndClearEvents(IEnumerable<BaseEntity> entitiesWithEvents);
    }
}
