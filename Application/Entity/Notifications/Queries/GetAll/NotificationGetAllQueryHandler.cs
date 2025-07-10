using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Notifications.Queries.GetAll
{
    internal class NotificationGetAllQueryHandler(INotificationRepository notificationRepository) : IQueryHandler<NotificationGetAllQuery, List<NotificationResponse>>
    {
        public async Task<Result<List<NotificationResponse>>> Handle(NotificationGetAllQuery request, CancellationToken cancellationToken)
        {
            var entities = request.StartIndex == null || request.Count == null
                ? await notificationRepository.GetAllAsync(request.Query.Predicate, cancellationToken)
                : await notificationRepository.GetAllAsync(request.StartIndex.Value, request.Count.Value, request.Query.Predicate, cancellationToken);
            if (entities.IsFailure) return Result.Failure<List<NotificationResponse>>(entities.Error);

            var listRes = entities.Value.Select(u => new NotificationResponse(u)).ToList();

            return listRes;
        }
    }
}
