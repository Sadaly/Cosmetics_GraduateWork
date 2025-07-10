using Application.Abstractions.Messaging;

namespace Application.Entity.PatientSpecificses.Commands.Create;
public sealed record PatientSpecificsCreateCommand(Guid PatientCardId, string Sleep, string Diet, string Sport, string WorkEnviroment) : ICommand<Guid>;
