using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Application.Contracts.Rooms;

public sealed record UpdateRoomRequest(
    string Number,
    RoomCategory Category,
    int MaxGuests,
    decimal BasePricePerNight,
    string? Description);
