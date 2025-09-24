using Application.Abstractions.Messaging;

namespace Application.Entity.ProcedureTypes.Commands.Create;
public sealed record ProcedureTypeCreateCommand(string Title, string Description, int StandartDur = 0, int StandartPrice = 0) : ICommand<Guid>;