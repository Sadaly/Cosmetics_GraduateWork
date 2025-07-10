using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Notifications.Queries.Get;
public sealed record NotificationGetQuery(EntityQueries<Notification> Query) : IQuery<NotificationResponse>;

