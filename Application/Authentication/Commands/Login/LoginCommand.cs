using InventorySystem.Domain.Common;
using MediatR;

namespace InventorySystem.Application.Authentication.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<Result<LoginResponse>>;

public record LoginResponse(string Token);
