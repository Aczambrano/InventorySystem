namespace InventorySystem.Application.Authentication;

public interface IJwtProvider
{
    string Generate(Guid userId, string username, string role);
}
