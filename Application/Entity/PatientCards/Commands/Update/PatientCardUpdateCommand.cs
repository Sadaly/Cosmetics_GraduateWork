using Application.Abstractions.Messaging;

namespace Application.Entity.PatientCards.Commands.Update;
public sealed record PatientCardUpdateCommand(Guid Id, byte? Age, string? Address, string? Complaints, string? PhoneNumber) : ICommand<Guid>;