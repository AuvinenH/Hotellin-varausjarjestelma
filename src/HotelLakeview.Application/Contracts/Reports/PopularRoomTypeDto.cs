using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Application.Contracts.Reports;

public sealed record PopularRoomTypeDto(
    RoomCategory Category,
    int ReservationCount,
    int NightCount);
