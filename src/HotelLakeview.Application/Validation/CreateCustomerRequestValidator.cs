using FluentValidation;
using HotelLakeview.Application.Contracts.Customers;

namespace HotelLakeview.Application.Validation;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);
    }
}
