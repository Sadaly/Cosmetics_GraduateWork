using Domain.Errors;
using Domain.Shared;

namespace Domain.Entity
{

	[Serializable]
	public class SessionToken
	{
		public enum ExpirationType
		{
			Unlimited = 0,
			Minutes = 1,
			Hours = 2,
			Days = 3,
			Months = 4,
			Years = 5,
		}
		private SessionToken(Guid id, Guid userId, string username, DateTime? expirationDateTime = null)
		{
			Id = id;
			UserId = userId;
			Username = username;
			ExpirationDateTime = expirationDateTime;
			CreationDateTime = DateTime.UtcNow;
		}

		public Guid Id { get; protected set; }
		public Guid UserId { get; protected set; }
		public string Username { get; protected set; } = null!;
		public DateTime CreationDateTime { get; protected set; }
		public DateTime? ExpirationDateTime { get; protected set; }
		public DateTime? RevokeDateTime { get; protected set; } = null;
		public bool IsRevoked { get; protected set; } = false;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user">Экземпляр сущности пользователя</param>
		/// <param name="expirationType">Указывает тип окончания токена. Определяет какую функцию использовать при добавлении времени истечения срока</param>
		/// <param name="expirationValue">Указывает какое количество времени будет добавлено к выбранному методу</param>
		/// <returns></returns>
		public Result<SessionToken> Create(Result<User> user, ExpirationType expirationType, int? expirationValue = null)
		{
			if (user.IsFailure) return Result.Failure<SessionToken>(user.Error);

			if (expirationType == ExpirationType.Unlimited && expirationValue != null) return DomainErrors.SessionToken.ExpirationParametersIncorrectWhenNull;
			if (expirationType != ExpirationType.Unlimited && expirationValue == null) return DomainErrors.SessionToken.ExpirationParametersIncorrectWhenNull;
			if (expirationType == ExpirationType.Unlimited && expirationValue == null) return new SessionToken(Guid.NewGuid(), user.Value.Id, user.Value.Username.Value);

			if (expirationType == ExpirationType.Minutes)	return new SessionToken(Guid.NewGuid(), user.Value.Id, user.Value.Username.Value, DateTime.UtcNow.AddMinutes(expirationValue!.Value));
			if (expirationType == ExpirationType.Hours)		return new SessionToken(Guid.NewGuid(), user.Value.Id, user.Value.Username.Value, DateTime.UtcNow.AddHours(expirationValue!.Value));
			if (expirationType == ExpirationType.Days)		return new SessionToken(Guid.NewGuid(), user.Value.Id, user.Value.Username.Value, DateTime.UtcNow.AddDays(expirationValue!.Value));
			if (expirationType == ExpirationType.Months)	return new SessionToken(Guid.NewGuid(), user.Value.Id, user.Value.Username.Value, DateTime.UtcNow.AddMonths(expirationValue!.Value));
			if (expirationType == ExpirationType.Years)		return new SessionToken(Guid.NewGuid(), user.Value.Id, user.Value.Username.Value, DateTime.UtcNow.AddYears(expirationValue!.Value));

			return DomainErrors.SessionToken.BigError;
		}
	}
}