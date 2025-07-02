using MediatR;
using Domain.Abstractions;

namespace Application.Abstractions.Messaging
{
    /// <summary>
    /// Обработчик Domain событий
    /// </summary>
    /// <typeparam name="TEvent">Обрабатываемое событие</typeparam>
    public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
        where TEvent : IDomainEvent
    {
    }
}
