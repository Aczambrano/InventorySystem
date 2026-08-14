using InventorySystem.Application.Data;
using InventorySystem.Domain.Common;
using InventorySystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Authentication.Commands.Register;

internal sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public RegisterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Users.AnyAsync(u => u.Username == request.Username, cancellationToken);
        if (exists)
        {
            return Result.Failure<Guid>(new Error("User.Duplicate", "Username already exists."));
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
