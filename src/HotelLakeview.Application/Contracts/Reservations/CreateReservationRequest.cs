namespace HotelLakeview.Application.Contracts.Reservations;

public sealed record CreateReservationRequest(
    Guid RoomId,
    Guid CustomerId,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    int GuestCount);
