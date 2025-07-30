using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class PatientRepository(AppDbContext dbContext,
        IPatientCardRepository patientCardRepository)
        : TRepository<Patient>(dbContext), IPatientRepository
    {
        protected readonly IPatientCardRepository _patientCardRepository = patientCardRepository;
        private protected override IQueryable<Patient> GetAllInclude()
            => base.GetAllInclude().Include(e => e.Card).Include(e => e.Card.SkinFeatures);
        public override async Task<Result<Patient>> RemoveAsync(Result<Patient> entity, CancellationToken cancellationToken = default)
        {
            if (entity.IsFailure) return entity;
            if (entity.Value.Card != null)
            {
                var card = await _patientCardRepository.GetByIdAsync(entity.Value.Card.Id, cancellationToken, Domain.Abstractions.FetchMode.Include);
                if (card.IsFailure) return Result.Failure<Patient>(card.Error);

                var remove = await _patientCardRepository.RemoveAsync(card, cancellationToken);
                if (remove.IsFailure) return Result.Failure<Patient>(remove.Error);
            }
            return await base.RemoveAsync(entity, cancellationToken);
        }
    }
}
