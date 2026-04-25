using HotelLakeview.Application.Contracts.Rooms;
using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Application.Abstractions;

public interface IRoomService
{
    Task<IReadOnlyList<RoomDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<RoomDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<RoomDto> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken);

    Task<RoomDto> UpdateAsync(Guid id, UpdateRoomRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<RoomDto>> GetAvailableAsync(DateOnly checkInDate, DateOnly checkOutDate, int guestCount, RoomCategory? category, CancellationToken cancellationToken);
}
