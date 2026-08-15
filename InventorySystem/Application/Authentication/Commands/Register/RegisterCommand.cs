using InventorySystem.Domain.Common;
using MediatR;

namespace InventorySystem.Application.Authentication.Commands.Register;

public record RegisterCommand(string Username, string Password, string Role) : IRequest<Result<Guid>>;
