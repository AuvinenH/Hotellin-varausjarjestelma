using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using HotelLakeview.Domain.ValueObjects;

namespace HotelLakeview.Application.Abstractions;

public interface IReservationRepository
{
    Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken cancellationToken);

    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> HasOverlapAsync(Guid roomId, DateRange dateRange, Guid? excludingReservationId, CancellationToken cancellationToken);

    Task AddAsync(Reservation reservation, IReadOnlyCollection<ReservationNight> nights, CancellationToken cancellationToken);

    Task<bool> TryUpdateAsync(Reservation reservation, IReadOnlyCollection<ReservationNight> nights, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<int> CountBookedNightsAsync(DateRange dateRange, CancellationToken cancellationToken);

    Task<IReadOnlyList<(int Year, int Month, decimal Revenue)>> GetMonthlyRevenueAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<(RoomCategory Category, int ReservationCount, int NightCount)>> GetPopularRoomTypesAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
}
