using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Application.Contracts.Reservations;

public sealed record ReservationDto(
    Guid Id,
    Guid RoomId,
    Guid CustomerId,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    int GuestCount,
    decimal TotalPrice,
    ReservationStatus Status,
    DateTime CreatedAtUtc);
