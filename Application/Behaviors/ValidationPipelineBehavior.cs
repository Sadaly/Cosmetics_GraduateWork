using Domain.Shared;
using FluentValidation;
using MediatR;

namespace Application.Behaviors
{
	public class ValidationPipelineBehavior<TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	where TResponse : Result
	{
		private readonly IEnumerable<IValidator<TRequest>> _validators;


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ValidationPipelineBehavior{TRequest, TResponse}"/>.
		/// </summary>
		/// <param name="validators">Коллекция валидаторов для текущего запроса.</param>
		public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators) =>
			_validators = validators;


		/// <summary>
		/// Обрабатывает запрос, выполняя его валидацию перед передачей следующему обработчику в конвейере.
		/// </summary>
		/// <param name="request">Запрос, который нужно обработать.</param>
		/// <param name="next">Делегат для вызова следующего обработчика в конвейере.</param>
		/// <param name="cancellationToken">Токен отмены.</param>
		/// <returns>Результат обработки запроса.</returns>
		public async Task<TResponse> Handle(
			TRequest request,
			RequestHandlerDelegate<TResponse> next,
			CancellationToken cancellationToken)
		{
			if (!_validators.Any())
			{
				return await next();
			}

			Error[] errors = _validators
				.Select(validator => validator.Validate(request))
				.SelectMany(validationResult => validationResult.Errors)
				.Where(validationFailure => validationFailure is not null)
				.Select(failure => new Error(
					failure.PropertyName,
					failure.ErrorMessage))
				.Distinct()
				.ToArray();

			if (errors.Any())
			{
				return CreateValidationResult<TResponse>(errors);
			}

			return await next();
		}


		/// <summary>
		/// Создает результат с ошибками валидации.
		/// </summary>
		/// <typeparam name="TResult">Тип результата, который должен быть производным от <see cref="Result"/>.</typeparam>
		/// <param name="errors">Массив ошибок валидации.</param>
		/// <returns>Результат с ошибками валидации.</returns>
		private static TResult CreateValidationResult<TResult>(Error[] errors)
			where TResult : Result
		{
			if (typeof(TResult) == typeof(Result))
			{
				return (ValidationResult.WithErrors(errors) as TResult)!;
			}

			object validationResult = typeof(ValidationResult<>)
				.GetGenericTypeDefinition()
				.MakeGenericType(typeof(TResult).GenericTypeArguments[0])
				.GetMethod(nameof(ValidationResult.WithErrors))!
				.Invoke(null, new object?[] { errors })!;

			return (TResult)validationResult;
		}
	}
}
