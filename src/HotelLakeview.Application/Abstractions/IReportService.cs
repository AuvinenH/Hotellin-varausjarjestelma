using HotelLakeview.Application.Contracts.Reports;

namespace HotelLakeview.Application.Abstractions;

public interface IReportService
{
    Task<OccupancyReportDto> GetOccupancyAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<MonthlyRevenueDto>> GetMonthlyRevenueAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<PopularRoomTypeDto>> GetPopularRoomTypesAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
}
