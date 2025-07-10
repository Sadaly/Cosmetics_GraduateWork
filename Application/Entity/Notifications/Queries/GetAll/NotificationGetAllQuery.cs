using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.Notifications.Queries.GetAll;
public sealed record NotificationGetAllQuery(
    EntityQueries<Notification> Query,
    int? StartIndex = null,
    int? Count = null) : IQuery<List<NotificationResponse>>;