namespace HotelLakeview.Application.Contracts.Reservations;

public sealed record UpdateReservationRequest(
    Guid RoomId,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    int GuestCount);
