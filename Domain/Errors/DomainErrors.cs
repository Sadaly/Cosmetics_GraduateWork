using Domain.Shared;
namespace Domain.Errors
{
	public static class DomainErrors
	{
		public static class SessionToken
		{
			public static readonly Error ExpirationDateTimeLessThenCurrentDateTime = new(
				"SessionToken.ExpirationDateTimeLessThenCurrentDateTime",
				"Невозможно установить дату и время окончания токена ниже текущей даты и времени");
			public static readonly Error ExpirationParametersIncorrectWhenNull = new(
				"SessionToken.ExpirationParametersIncorrectWhenNull",
				"Одно из значений равно null, но второе почему-то имеет вещественное значение");
			public static readonly Error BigError = new(
				"SessionToken.BigError",
				"Что-то пошло не так");
		}
		public static class Notification
		{
			public static readonly Error Late = new(
				"Notification.Late",
				"Дата отправки позже чем, дата самой процедуры");

			public static readonly Error ProcedureNotScheduled = new(
				"Notification.ProcedureNotScheduled",
				"Прежде чем создать уведомление нужно назначить дату процедуры");
		}
		public static class Procedure
		{
			public static readonly Error DurationLessThenZero = new(
				"Procedure.DurationLessThenZero",
				"Длительность процедуры не может быть ниже 0");
			public static readonly Error AlreadyNoDoctor = new(
				"Procedure.AlreadyNoDoctor",
				"На процедуру и так не назначен доктор");
		}
		public static class Username
		{
			public static readonly Error Empty = new(
				"Username.Empty",
				"Пустое поле");

			public static readonly Error TooLong = new(
				"Username.TooLong",
				$"Поле слишком длинное. Максимальная длина {Domain.ValueObjects.Username.MAX_LENGTH} символов");

			public static readonly Error TooShort = new(
				"Username.TooShort",
				$"Поле слишком короткое. Минимальная длина {Domain.ValueObjects.Username.MAX_LENGTH} символа");

			public static readonly Error AlreadySet = new(
				"Username.AlreadySet",
				"Имя пользователя совпадает с предыдущим");
		}

		public static class Email
		{
			public static readonly Error Empty = new(
				"Email.Empty",
				"Пустое поле");

			public static readonly Error TooLong = new(
				"Email.TooLong",
				$"Поле слишком длинное. Максимальная длина {Domain.ValueObjects.Email.MAX_LENGTH} символов");

			public static readonly Error TooShort = new(
				"Email.TooShort",
				$"Поле слишком короткое. Минимальная длина {Domain.ValueObjects.Email.MIN_LENGTH} символа");

			public static readonly Error InvalidFormat = new(
				"Email.InvalidFormat",
				$"Поле имеет неправильный формат. Верный формат: example@mail.com");

			public static readonly Error AlreadySet = new(
				"Email.AlreadySet",
				"Значение совпадает с предыдущим");
		}

		public static class PasswordHashed
		{
			public static readonly Error Empty = new(
				"PasswordHashed.Empty",
				"Пустое поле");

			public static readonly Error TooLong = new(
				"PasswordHashed.TooLong",
				$"Поле слишком длинное. Максимальная длина {Domain.ValueObjects.PasswordHashed.MAX_LENGTH} символов");

			public static readonly Error TooShort = new(
				"PasswordHashed.TooShort",
				$"Поле слишком короткое. Минимальная длина {Domain.ValueObjects.PasswordHashed.MAX_LENGTH} символа");

			public static readonly Error AlreadySet = new(
				"PasswordHashed.AlreadySet",
				"Значение совпадает с предыдущим");
		}

		public static class PhoneNumber
		{
			public static readonly Error Empty = new(
				"PhoneNumber.Empty",
				"Телефон пуст");

			public static readonly Error TooLong = new(
				"PhoneNumber.TooLong",
				"Телефон слишком длинный");

			public static readonly Error TooShort = new(
				"PhoneNumber.TooLong",
				"Телефон слишком короткий");

			public static readonly Error InvalidFormat = new(
				"PhoneNumber.InvalidFormat",
				"Телефон имеет неверный формат или размер");

			public static readonly Error AlreadyVerified = new(
				"PhoneNumber.AlreadyVerified",
				"Телефон уже подтвержден");

			public static readonly Error AlreadySet = new(
				"PhoneNumber.AlreadySet",
				"Значение совпадает с предыдущим");
		}

		public static class Text
		{
			public static readonly Error Empty = new(
				"Text.Empty",
				"Содержание пусто");

			public static readonly Error TooLong = new(
				"Text.TooLong",
				"Содержание слишком длинное");

			public static readonly Error AlreadySet = new(
				"Text.AlreadySet",
				"Значение совпадает с предыдущим");
		}

		public static class Title
		{
			public static readonly Error Empty = new(
				"Title.Empty",
				"Название пусто");

			public static readonly Error TooLong = new(
				"Title.TooLong",
				"Название слишком длинное");

			public static readonly Error AlreadySet = new(
				"Title.AlreadySet",
				"Значение совпадает с предыдущим");
		}
	}
}
