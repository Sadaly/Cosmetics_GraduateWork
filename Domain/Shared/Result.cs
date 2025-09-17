using Domain.Common;

namespace Domain.Shared
{
	/// <summary>
	/// Класс для представления результата операции.
	/// </summary>
	public class Result
	{
		/// <summary>
		/// Внутренний конструктор для создания результата.
		/// </summary>
		/// <param name="isSuccess">Указывает, была ли операция успешной.</param>
		/// <param name="error">Ошибка, если операция завершилась неудачно.</param>
		/// <exception cref="InvalidOperationException">
		/// Выбрасывается, если:
		/// - Операция успешна, но ошибка не равна Error.None.
		/// - Операция неудачна, но ошибка равна Error.None.
		/// </exception>
		protected internal Result(bool isSuccess, Error error)
		{
			if (isSuccess && error != Error.None)
			{
				throw new InvalidOperationException("Успешный результат не может содержать ошибку.");
			}

			if (!isSuccess && error == Error.None)
			{
				throw new InvalidOperationException("Неудачный результат должен содержать ошибку.");
			}

			IsSuccess = isSuccess;
			Error = error;
		}

		/// <summary>
		/// Указывает, была ли операция успешной.
		/// </summary>
		public bool IsSuccess { get; }

		/// <summary>
		/// Указывает, завершилась ли операция ошибкой.
		/// </summary>
		public bool IsFailure => !IsSuccess;

		/// <summary>
		/// Ошибка, если операция завершилась неудачно.
		/// </summary>
		public Error Error { get; }

		/// <summary>
		/// Создает успешный результат без значения.
		/// </summary>
		/// <returns>Успешный результат.</returns>
		public static Result Success() => new(true, Error.None);

		/// <summary>
		/// Создает успешный результат с указанным значением.
		/// </summary>
		/// <typeparam name="TValue">Тип значения.</typeparam>
		/// <param name="value">Значение результата.</param>
		/// <returns>Успешный результат с значением.</returns>
		public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

		/// <summary>
		/// Создает неудачный результат с указанной ошибкой.
		/// </summary>
		/// <param name="error">Ошибка.</param>
		/// <returns>Неудачный результат.</returns>
		public static Result Failure(Error error) => new(false, error);
		/// <summary>
		/// Создает неудачный результат на основе переданного результата.
		/// </summary>
		/// <param name="result">результат.</param>
		/// <returns>Неудачный результат.</returns>
		public static Result Failure(Result result)
		{
			if (result.IsSuccess) throw new Exception("Big error! попытка присвоить неудачному результату значение из удачного результата");
			return Failure(result.Error);
		}

		/// <summary>
		/// Создает неудачный результат с указанной ошибкой (для обобщенного результата).
		/// </summary>
		/// <typeparam name="TValue">Тип значения.</typeparam>
		/// <param name="error">Ошибка.</param>
		/// <returns>Неудачный результат с ошибкой.</returns>
		public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

		/// <summary>
		/// Создает неудачный результат на основе переданного результата (для обобщенного результата).
		/// </summary>
		/// <typeparam name="TValue">Тип значения.</typeparam>
		/// <param name="result">Результат</param>
		/// <returns>Неудачный результат с ошибкой.</returns>
		public static Result<TValue> Failure<TValue>(Result result) => Failure<TValue>(result.Error);

		/// <summary>
		/// Создает результат на основе переданного значения.
		/// Если значение null, возвращает неудачный результат с ошибкой Error.NullValue.
		/// </summary>
		/// <typeparam name="TValue">Тип значения.</typeparam>
		/// <param name="value">Значение.</param>
		/// <returns>Результат с значением или ошибкой.</returns>
		public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

		public static implicit operator Result(Error error) => Failure(error);
	}
}