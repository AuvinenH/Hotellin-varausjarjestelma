using FluentValidation;
using HotelLakeview.Application.Contracts.Reservations;

namespace HotelLakeview.Application.Validation;

public class UpdateReservationRequestValidator : AbstractValidator<UpdateReservationRequest>
{
    public UpdateReservationRequestValidator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty();

        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate);

        RuleFor(x => x.GuestCount)
            .GreaterThan(0)
            .LessThanOrEqualTo(10);
    }
}
