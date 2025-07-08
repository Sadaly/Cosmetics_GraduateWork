using Domain.Entity;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class NotificationRepository(AppDbContext dbContext) 
        : TRepository<Notification>(dbContext), INotificationRepository
    {
        private protected override IQueryable<Notification> GetAllInclude()
            => base.GetAllInclude()
            .Include(e => e.Procedure);
    }
}
