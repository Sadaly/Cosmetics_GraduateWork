using Domain.Shared;
using MediatR;

namespace Application.Abstractions.Messaging
{
	/// <summary>
	/// Запрос на вытягивание
	/// </summary>
	/// <typeparam name="TResponse">Возвращаемый результат запроса</typeparam>
	public interface IQuery<TResponse> : IRequest<Result<TResponse>>
	{
	}
}
