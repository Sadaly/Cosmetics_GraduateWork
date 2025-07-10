using Application.Abstractions.Messaging;

namespace Application.Entity.PatientSpecificses.Commands.Update;
public sealed record PatientSpecificsUpdateCommand(Guid Id, string Sleep, string Diet, string Sport, string WorkEnviroment) : ICommand<Guid>;