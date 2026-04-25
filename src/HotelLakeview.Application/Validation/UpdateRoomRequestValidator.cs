using FluentValidation;
using HotelLakeview.Application.Contracts.Rooms;

namespace HotelLakeview.Application.Validation;

public class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
{
    public UpdateRoomRequestValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.MaxGuests)
            .GreaterThan(0)
            .LessThanOrEqualTo(10);

        RuleFor(x => x.BasePricePerNight)
            .GreaterThan(0);

        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}
