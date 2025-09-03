using Domain.Shared;
using MediatR;

namespace Application.Abstractions.Messaging
{
	/// <summary>
	/// Интерфейс для команд с возвращением информации об успехе выполнения команды
	/// </summary>
	public interface ICommand : IRequest<Result>
	{
	}
	/// <summary>
	/// Интерфейс для команд с возвращением информации об успехе выполнения команды и результатом выполнения
	/// </summary>
	/// <typeparam name="TResponse"></typeparam>
	public interface ICommand<TResponse> : IRequest<Result<TResponse>>
	{
	}
}
