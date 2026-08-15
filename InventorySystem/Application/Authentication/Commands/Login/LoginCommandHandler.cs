using InventorySystem.Application.Data;
using InventorySystem.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Authentication.Commands.Login;

internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(IApplicationDbContext context, IJwtProvider jwtProvider)
    {
        _context = context;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user is null)
        {
            return Result.Failure<LoginResponse>(DomainErrors.User.InvalidCredentials);
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<LoginResponse>(DomainErrors.User.InvalidCredentials);
        }

        var token = _jwtProvider.Generate(user.Id, user.Username, user.Role);

        return Result.Success(new LoginResponse(token));
    }
}
