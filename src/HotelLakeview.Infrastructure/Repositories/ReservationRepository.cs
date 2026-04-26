using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Exceptions;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using HotelLakeview.Domain.ValueObjects;
using HotelLakeview.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HotelLakeview.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly HotelDbContext _dbContext;

    public ReservationRepository(HotelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Reservations
            .AsNoTracking()
            .OrderByDescending(reservation => reservation.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Reservations.FirstOrDefaultAsync(reservation => reservation.Id == id, cancellationToken);
    }

    public Task<bool> HasOverlapAsync(Guid roomId, DateRange dateRange, Guid? excludingReservationId, CancellationToken cancellationToken)
    {
        return _dbContext.ReservationNights.AnyAsync(night =>
                night.RoomId == roomId
                && night.NightDate >= dateRange.CheckInDate
                && night.NightDate < dateRange.CheckOutDate
                && night.Reservation!.Status != ReservationStatus.Cancelled
                && (!excludingReservationId.HasValue || night.ReservationId != excludingReservationId.Value),
            cancellationToken);
    }

    public async Task AddAsync(Reservation reservation, IReadOnlyCollection<ReservationNight> nights, CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.Reservations.Add(reservation);
            _dbContext.ReservationNights.AddRange(nights);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new ConflictException("Selected room is already booked for one or more nights.");
        }
    }

    public async Task<bool> TryUpdateAsync(Reservation reservation, IReadOnlyCollection<ReservationNight> nights, CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var existingNights = await _dbContext.ReservationNights
            .Where(night => night.ReservationId == reservation.Id)
            .ToListAsync(cancellationToken);

        _dbContext.ReservationNights.RemoveRange(existingNights);
        _dbContext.ReservationNights.AddRange(nights);
        _dbContext.Reservations.Update(reservation);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CountBookedNightsAsync(DateRange dateRange, CancellationToken cancellationToken)
    {
        return _dbContext.ReservationNights.CountAsync(night =>
            night.NightDate >= dateRange.CheckInDate
            && night.NightDate < dateRange.CheckOutDate
            && night.Reservation!.Status != ReservationStatus.Cancelled,
            cancellationToken);
    }

    public async Task<IReadOnlyList<(int Year, int Month, decimal Revenue)>> GetMonthlyRevenueAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        var rows = await _dbContext.Reservations
            .AsNoTracking()
            .Where(reservation =>
                reservation.Status != ReservationStatus.Cancelled
                && reservation.CheckInDate >= startDate
                && reservation.CheckInDate < endDate)
            .Select(reservation => new
            {
                reservation.CheckInDate.Year,
                reservation.CheckInDate.Month,
                reservation.TotalPrice,
            })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(item => new { item.Year, item.Month })
            .Select(group =>
            {
                var revenue = group.Sum(item => item.TotalPrice);
                return (group.Key.Year, group.Key.Month, revenue);
            })
            .ToList();
    }

    public async Task<IReadOnlyList<(RoomCategory Category, int ReservationCount, int NightCount)>> GetPopularRoomTypesAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        var rows = await _dbContext.Reservations
            .AsNoTracking()
            .Where(reservation =>
                reservation.Status != ReservationStatus.Cancelled
                && reservation.CheckInDate >= startDate
                && reservation.CheckInDate < endDate)
            .Join(
                _dbContext.Rooms,
                reservation => reservation.RoomId,
                room => room.Id,
                (reservation, room) => new
                {
                    room.Category,
                    reservation.CheckInDate,
                    reservation.CheckOutDate,
                })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(item => item.Category)
            .Select(group =>
            {
                var reservationCount = group.Count();
                var nightCount = group.Sum(item => item.CheckOutDate.DayNumber - item.CheckInDate.DayNumber);
                return (group.Key, reservationCount, nightCount);
            })
            .ToList();
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is SqliteException sqliteException
            && sqliteException.SqliteErrorCode == 19;
    }
}
