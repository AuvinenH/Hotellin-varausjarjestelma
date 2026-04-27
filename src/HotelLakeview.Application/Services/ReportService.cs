using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.Contracts.Reports;
using HotelLakeview.Domain.ValueObjects;

namespace HotelLakeview.Application.Services;

public class ReportService : IReportService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;

    public ReportService(IReservationRepository reservationRepository, IRoomRepository roomRepository)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
    }

    public async Task<Result<OccupancyReportDto>> GetOccupancyAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        DateRange dateRange;
        try
        {
            dateRange = new DateRange(startDate, endDate);
        }
        catch (ArgumentException exception)
        {
            return Result<OccupancyReportDto>.Failure(ResultError.Validation("report.invalid_date_range", exception.Message));
        }

        var totalRooms = await _roomRepository.CountAsync(cancellationToken);
        var reservedNights = await _reservationRepository.CountBookedNightsAsync(dateRange, cancellationToken);
        var totalRoomNights = totalRooms * dateRange.Nights;

        return Result<OccupancyReportDto>.Success(new OccupancyReportDto(
            startDate,
            endDate,
            reservedNights,
            totalRoomNights,
            OccupancyCalculator.CalculateOccupancyRatePercent(reservedNights, totalRoomNights)));
    }

    public async Task<Result<IReadOnlyList<MonthlyRevenueDto>>> GetMonthlyRevenueAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        try
        {
            _ = new DateRange(startDate, endDate);
        }
        catch (ArgumentException exception)
        {
            return Result<IReadOnlyList<MonthlyRevenueDto>>.Failure(ResultError.Validation("report.invalid_date_range", exception.Message));
        }

        var data = await _reservationRepository.GetMonthlyRevenueAsync(startDate, endDate, cancellationToken);

        return Result<IReadOnlyList<MonthlyRevenueDto>>.Success(data
            .Select(item => new MonthlyRevenueDto(item.Year, item.Month, item.Revenue))
            .OrderBy(item => item.Year)
            .ThenBy(item => item.Month)
            .ToList());
    }

    public async Task<Result<IReadOnlyList<PopularRoomTypeDto>>> GetPopularRoomTypesAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        try
        {
            _ = new DateRange(startDate, endDate);
        }
        catch (ArgumentException exception)
        {
            return Result<IReadOnlyList<PopularRoomTypeDto>>.Failure(ResultError.Validation("report.invalid_date_range", exception.Message));
        }

        var data = await _reservationRepository.GetPopularRoomTypesAsync(startDate, endDate, cancellationToken);

        return Result<IReadOnlyList<PopularRoomTypeDto>>.Success(data
            .Select(item => new PopularRoomTypeDto(item.Category, item.ReservationCount, item.NightCount))
            .OrderByDescending(item => item.NightCount)
            .ToList());
    }
}
