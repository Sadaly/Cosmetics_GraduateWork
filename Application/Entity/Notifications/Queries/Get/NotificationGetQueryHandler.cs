using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Entity.Notifications.Queries.Get
{
	internal class NotificationGetQueryHandler(INotificationRepository notificationRepository) : IQueryHandler<NotificationGetQuery, NotificationResponse>
	{
		public async Task<Result<NotificationResponse>> Handle(NotificationGetQuery request, CancellationToken cancellationToken)
		{
			var entity = await notificationRepository.GetByPredicateAsync(request.Query.Predicate, cancellationToken);
			if (entity.IsFailure) return Result.Failure<NotificationResponse>(entity.Error);

			var response = new NotificationResponse(entity.Value);

			return response;
		}
	}
}
