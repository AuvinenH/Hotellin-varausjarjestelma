using HotelLakeview.Application.Contracts.Rooms;
using HotelLakeview.Application.Common;
using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Application.Abstractions;

public interface IRoomService
{
    Task<Result<IReadOnlyList<RoomDto>>> GetAllAsync(CancellationToken cancellationToken);

    Task<Result<RoomDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<RoomDto>> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken);

    Task<Result<RoomDto>> UpdateAsync(Guid id, UpdateRoomRequest request, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<RoomDto>>> GetAvailableAsync(DateOnly checkInDate, DateOnly checkOutDate, int guestCount, RoomCategory? category, CancellationToken cancellationToken);
}
