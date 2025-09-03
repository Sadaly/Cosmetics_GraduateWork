using Domain.Shared;
using MediatR;

namespace Application.Abstractions.Messaging
{
	/// <summary>
	/// Интерфейс обработчика команд
	/// </summary>
	/// <typeparam name="TCommand">Выполненная команда</typeparam>
	public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
	 where TCommand : ICommand
	{
	}

	/// <summary>
	/// Интерфейс обработчика команд с возвращаемым результатом
	/// </summary>
	/// <typeparam name="TCommand">Выполненная команда</typeparam>
	/// <typeparam name="TResponse">Полученный результат</typeparam>
	public interface ICommandHandler<TCommand, TResponse>
		: IRequestHandler<TCommand, Result<TResponse>>
		where TCommand : ICommand<TResponse>
	{
	}
}
