using Domain.Abstractions;
using Domain.Common;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class PatientCardRepository(AppDbContext dbContext,
        IAgeChangeRepository ageChangeRepository,
        IExternalProcedureRecordRepository externalProcedureRecordRepository,
        IHealthCondRepository healthCondRepository,
        IProcedureRepository procedureRepository,
        ISkinCareRepository skinCareRepository,
        ISkinFeatureRepository skinFeatureRepository,
        IPatientSpecificsRepository patientSpecificsRepository)
        : TRepository<PatientCard>(dbContext), IPatientCardRepository
    {
        protected readonly IAgeChangeRepository _ageChangeRepository = ageChangeRepository;
        protected readonly IExternalProcedureRecordRepository _externalProcedureRecordRepository = externalProcedureRecordRepository;
        protected readonly IHealthCondRepository _healthCondRepository = healthCondRepository;
        protected readonly IProcedureRepository _procedureRepository = procedureRepository;
        protected readonly ISkinCareRepository _skinCareRepository = skinCareRepository;
        protected readonly ISkinFeatureRepository _skinFeatureRepository = skinFeatureRepository;
        protected readonly IPatientSpecificsRepository _patientSpecificsRepository = patientSpecificsRepository;

        private protected override IQueryable<PatientCard> GetAllInclude()
            => base.GetAllInclude()
            .Include(e => e.AgeChanges)
            .Include(e => e.ExternalProcedureRecords)
            .Include(e => e.HealthConds)
            .Include(e => e.Patient)
            .Include(e => e.Procedures)
            .Include(e => e.SkinCares)
            .Include(e => e.SkinFeatures)
            .Include(e => e.Specifics);

        public override async Task<Result<PatientCard>> RemoveAsync(Result<PatientCard> entity, CancellationToken cancellationToken = default)
        {
            if (entity.IsFailure) return entity;

            var remove = await RemoveInnerListAsync<AgeChange>(entity, _ageChangeRepository, x => x.PatientCardId == entity.Value.Id, cancellationToken);
            if (remove.IsFailure) return remove;

            remove = await RemoveInnerListAsync<ExternalProcedureRecord>(entity, _externalProcedureRecordRepository, x => x.PatientCardId == entity.Value.Id, cancellationToken);
            if (remove.IsFailure) return remove;

            remove = await RemoveInnerListAsync<HealthCond>(entity, _healthCondRepository, x => x.PatientCardId == entity.Value.Id, cancellationToken);
            if (remove.IsFailure) return remove;

            remove = await RemoveInnerListAsync<Procedure>(entity, _procedureRepository, x => x.PatientCardId == entity.Value.Id, cancellationToken);
            if (remove.IsFailure) return remove;

            remove = await RemoveInnerListAsync<SkinCare>(entity, _skinCareRepository, x => x.PatientCardId == entity.Value.Id, cancellationToken);
            if (remove.IsFailure) return remove;

            remove = await RemoveInnerListAsync<SkinFeature>(entity, _skinFeatureRepository, x => x.PatientCardId == entity.Value.Id, cancellationToken);
            if (remove.IsFailure) return remove;

            remove = await RemoveInnerListAsync<PatientSpecifics>(entity, _patientSpecificsRepository, x => x.PatientCardId == entity.Value.Id, cancellationToken);
            if (remove.IsFailure) return remove;

            return await base.RemoveAsync(entity, cancellationToken);
        }

        private async Task<Result<PatientCard>> RemoveInnerListAsync<T>(Result<PatientCard> entity, IRepository<T> _repository, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
            where T : BaseEntity
        {
            if (entity.IsFailure) return entity;
            var eList = await _repository.GetAllAsync(predicate, cancellationToken, FetchMode.Include);
            if (eList.IsFailure) return Result.Failure<PatientCard>(eList.Error);
            foreach (var e in eList.Value)
            {
                var remove = await _repository.RemoveAsync(e, cancellationToken);
                if (remove.IsFailure) return Result.Failure<PatientCard>(remove.Error);
            }
            return entity;
        }
    }
}
