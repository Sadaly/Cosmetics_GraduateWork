using Application.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Application.Entity.Notifications.Queries;

public sealed record NotificationQueries(Expression<Func<Notification, bool>> Predicate) : EntityQueries<Notification>(Predicate)
{
}
