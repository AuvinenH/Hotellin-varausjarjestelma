using HotelLakeview.Application.Contracts.Reports;
using HotelLakeview.Application.Common;

namespace HotelLakeview.Application.Abstractions;

public interface IReportService
{
    Task<Result<OccupancyReportDto>> GetOccupancyAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<MonthlyRevenueDto>>> GetMonthlyRevenueAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<PopularRoomTypeDto>>> GetPopularRoomTypesAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
}
