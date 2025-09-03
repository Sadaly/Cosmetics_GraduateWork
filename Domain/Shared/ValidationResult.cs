namespace Domain.Shared;

/// <summary>
/// Класс для представления результата валидации.
/// </summary>
public sealed class ValidationResult : Result, IValidationResult
{
	/// <summary>
	/// Приватный конструктор для создания результата валидации.
	/// </summary>
	/// <param name="errors">Массив ошибок валидации.</param>
	private ValidationResult(Error[] errors)
		: base(false, IValidationResult.ValidationError) // Передаем false и стандартную ошибку валидации
		=> Errors = errors;

	/// <summary>
	/// Массив ошибок валидации.
	/// </summary>
	public Error[] Errors { get; }

	/// <summary>
	/// Создает новый объект ValidationResult с указанным массивом ошибок.
	/// </summary>
	/// <param name="errors">Массив ошибок валидации.</param>
	/// <returns>Результат валидации с ошибками.</returns>
	public static ValidationResult WithErrors(Error[] errors) => new(errors);
}