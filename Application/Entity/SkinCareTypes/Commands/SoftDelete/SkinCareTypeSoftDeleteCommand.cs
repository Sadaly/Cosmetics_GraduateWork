using Application.Abstractions.Messaging;

namespace Application.Entity.SkinCareTypes.Commands.SoftDelete;
public sealed record SkinCareTypeSoftDeleteCommand(Guid Id) : ICommand<Guid>;