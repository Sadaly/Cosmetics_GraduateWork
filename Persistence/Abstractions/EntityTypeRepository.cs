using Domain.Abstractions;
using Domain.Common;
using Domain.Errors;
using Domain.Shared;

namespace Persistence.Abstractions
{
	public abstract class EntityTypeRepository<TypeE, EntityWithT>(AppDbContext dbContext, IEntityWithTypeRepository<TypeE, EntityWithT> EntityWithTypeRepository)
		: TRepository<TypeE>(dbContext), IEntityTypeRepository<TypeE, EntityWithT>
		where TypeE : TypeEntity
		where EntityWithT : EntityWithType<TypeE>
	{
		protected readonly IEntityWithTypeRepository<TypeE, EntityWithT> _EntityWithTypeRepository = EntityWithTypeRepository;

		public override async Task<Result<TypeE>> RemoveAsync(Result<TypeE> entity, CancellationToken cancellationToken = default)
		{
			if (entity.IsFailure) return entity;
			var teList = await _EntityWithTypeRepository.GetAllAsync(te => te.TypeId == entity.Value.Id, cancellationToken);
			if (teList.IsFailure) return Result.Failure<TypeE>(teList.Error);
			foreach (var te in teList.Value)
				await _EntityWithTypeRepository.RemoveAsync(te, cancellationToken);
			return await base.RemoveAsync(entity, cancellationToken);
		}

		protected override async Task<Result<TypeE>> VerificationBeforeAddingAsync(Result<TypeE> entity, CancellationToken cancellationToken = default)
		{
			var base_verific = await base.VerificationBeforeAddingAsync(entity, cancellationToken);
			if (base_verific.IsFailure) return base_verific;

			//Запрещаем создавать сущности с уже используемыми названиями
			var exists = await GetByPredicateAsync(title => title.Title == entity.Value.Title, cancellationToken);
			if (!exists.IsFailure) return PersistenceErrors.TypeEntity<TypeE>.TitleAlreadyInUse;

			return entity.Value;
		}

		protected override async Task<Result<TypeE>> VerificationBeforeUpdateAsync(Result<TypeE> entity, CancellationToken cancellationToken = default)
		{
			var base_verific = await base.VerificationBeforeUpdateAsync(entity, cancellationToken);
			if (base_verific.IsFailure) return base_verific;

			//Запрещаем создавать сущности с уже используемыми названиями
			var exists = await GetByPredicateAsync(title => title.Title == entity.Value.Title, cancellationToken, FetchMode.NoTracking);
			if (!exists.IsFailure) return PersistenceErrors.TypeEntity<TypeE>.TitleAlreadyInUse;

			return entity.Value;
		}
	}
}
