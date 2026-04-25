using FluentValidation;
using HotelLakeview.Application.Contracts.Customers;

namespace HotelLakeview.Application.Validation;

public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
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
