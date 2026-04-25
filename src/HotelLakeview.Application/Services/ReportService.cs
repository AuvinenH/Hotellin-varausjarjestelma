using HotelLakeview.Application.Abstractions;
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

    public async Task<OccupancyReportDto> GetOccupancyAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        var dateRange = new DateRange(startDate, endDate);
        var totalRooms = await _roomRepository.CountAsync(cancellationToken);
        var reservedNights = await _reservationRepository.CountBookedNightsAsync(dateRange, cancellationToken);
        var totalRoomNights = totalRooms * dateRange.Nights;

        return new OccupancyReportDto(
            startDate,
            endDate,
            reservedNights,
            totalRoomNights,
            OccupancyCalculator.CalculateOccupancyRatePercent(reservedNights, totalRoomNights));
    }

    public async Task<IReadOnlyList<MonthlyRevenueDto>> GetMonthlyRevenueAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        _ = new DateRange(startDate, endDate);
        var data = await _reservationRepository.GetMonthlyRevenueAsync(startDate, endDate, cancellationToken);

        return data
            .Select(item => new MonthlyRevenueDto(item.Year, item.Month, item.Revenue))
            .OrderBy(item => item.Year)
            .ThenBy(item => item.Month)
            .ToList();
    }

    public async Task<IReadOnlyList<PopularRoomTypeDto>> GetPopularRoomTypesAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        _ = new DateRange(startDate, endDate);
        var data = await _reservationRepository.GetPopularRoomTypesAsync(startDate, endDate, cancellationToken);

        return data
            .Select(item => new PopularRoomTypeDto(item.Category, item.ReservationCount, item.NightCount))
            .OrderByDescending(item => item.NightCount)
            .ToList();
    }
}
