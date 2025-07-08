using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class ProcedureRepository(AppDbContext dbContext,
        INotificationRepository notificationRepository)
        : EntityWithTypeRepository<ProcedureType, Procedure>(dbContext), IProcedureRepository
    {
        protected readonly INotificationRepository _notificationRepository = notificationRepository;
        private protected override IQueryable<Procedure> GetAllInclude()
            => base.GetAllInclude()
            .Include(e => e.Doctor)
            .Include(e => e.Notification)
            .Include(e => e.PatientCard)
            .Include(e => e.Type);

        public override async Task<Result<Procedure>> RemoveAsync(Result<Procedure> entity, CancellationToken cancellationToken = default)
        {
            if (entity.IsFailure) return entity;
            var notific = await _notificationRepository.GetByPredicateAsync(x => x.ProcedureId == entity.Value.Id, cancellationToken);

            if (notific.IsFailure) return Result.Failure<Procedure>(notific.Error);

            var remove = await _notificationRepository.RemoveAsync(notific, cancellationToken);
            if (remove.IsFailure) return Result.Failure<Procedure>(remove.Error);

            return await base.RemoveAsync(entity, cancellationToken);
        }
    }
}