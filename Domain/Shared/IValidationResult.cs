namespace Domain.Shared;

public interface IValidationResult
{
	static readonly Error ValidationError = new(
		"ValidationError",
		"Возникла ошибка валидации значения.");

	Error[] Errors { get; }
}
