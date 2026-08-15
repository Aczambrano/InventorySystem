using FluentValidation;

namespace InventorySystem.Application.Products.Commands.Create;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(v => v.SKU)
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.")
            .NotEmpty().WithMessage("SKU is required.");

        RuleFor(v => v.ProviderId)
            .NotEmpty().WithMessage("Provider is required.");

        RuleFor(v => v.LotNumber)
            .MaximumLength(50).WithMessage("LotNumber must not exceed 50 characters.");

        RuleFor(v => v.UnitPrice)
            .GreaterThan(0).WithMessage("UnitPrice must be greater than zero.");

        RuleFor(v => v.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("StockQuantity cannot be negative.");

        RuleFor(v => v.ExpirationDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("ExpirationDate must be in the future.")
            .When(v => v.ExpirationDate.HasValue);
    }
}
