using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InventorySystem.Domain.Common;

public static class DomainErrors
{
    public static class User
    {
        public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "The email or password is incorrect.");
    }

    public static class Product
    {
        public static readonly Error NotFound = new("Product.NotFound", "The product with the specified ID was not found.");
    }
}
