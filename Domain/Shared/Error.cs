namespace Domain.Shared
{
	/// <summary>
	/// Создает новый экземпляр ошибки.
	/// </summary>
	/// <param name="code">Код ошибки.</param>
	/// <param name="message">Сообщение об ошибке.</param>
	public class Error(string code, string message) : IEquatable<Error>
	{
		/// <summary>
		/// Представляет отсутствие ошибки.
		/// </summary>
		public static readonly Error None = new(string.Empty, string.Empty);

		/// <summary>
		/// Стандартная ошибка, указывающая на null-значение.
		/// </summary>
		public static readonly Error NullValue = new("Error.NullValue", "Результат равен null.");

		/// <summary>
		/// Код ошибки.
		/// </summary>
		public string Code { get; } = code;

		/// <summary>
		/// Сообщение об ошибке.
		/// </summary>
		public string Message { get; } = message;

		/// <summary>
		/// Неявное преобразование ошибки в строку (возвращает код ошибки).
		/// </summary>
		/// <param name="error">Ошибка для преобразования.</param>
		public static implicit operator string(Error error) => error.Code;

		/// <summary>
		/// Оператор равенства для сравнения двух ошибок.
		/// </summary>
		/// <param name="a">Первая ошибка.</param>
		/// <param name="b">Вторая ошибка.</param>
		/// <returns>True, если ошибки равны; иначе False.</returns>
		public static bool operator ==(Error? a, Error? b)
		{
			if (a is null && b is null)
			{
				return true;
			}

			if (a is null || b is null)
			{
				return false;
			}

			return a.Equals(b);
		}

		/// <summary>
		/// Оператор неравенства для сравнения двух ошибок.
		/// </summary>
		/// <param name="a">Первая ошибка.</param>
		/// <param name="b">Вторая ошибка.</param>
		/// <returns>True, если ошибки не равны; иначе False.</returns>
		public static bool operator !=(Error? a, Error? b) => !(a == b);

		/// <summary>
		/// Сравнивает текущий объект ошибки с другим объектом ошибки.
		/// </summary>
		/// <param name="other">Другая ошибка для сравнения.</param>
		/// <returns>True, если ошибки равны; иначе False.</returns>
		public virtual bool Equals(Error? other)
		{
			if (other is null)
			{
				return false;
			}

			return Code == other.Code && Message == other.Message;
		}

		/// <summary>
		/// Сравнивает текущий объект ошибки с другим объектом.
		/// </summary>
		/// <param name="obj">Другой объект для сравнения.</param>
		/// <returns>True, если объекты равны; иначе False.</returns>
		public override bool Equals(object? obj) => obj is Error error && Equals(error);

		/// <summary>
		/// Возвращает хэш-код объекта на основе кода и сообщения.
		/// </summary>
		/// <returns>Хэш-код объекта.</returns>
		public override int GetHashCode() => HashCode.Combine(Code, Message);

		/// <summary>
		/// Возвращает строковое представление ошибки (обычно это код ошибки).
		/// </summary>
		/// <returns>Код ошибки.</returns>
		public override string ToString() => Code;
		/// <summary>
		/// Возвращает строковое представление ошибки.
		/// </summary>
		public string ToString(bool withMessage)
		{
			return Code + (withMessage ? " - " + Message : "");
		}
	}
}
