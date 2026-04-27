using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.Contracts.Reports;
using MediatR;

namespace HotelLakeview.Application.CQRS.Reports;

public sealed record GetOccupancyReportQuery(DateOnly StartDate, DateOnly EndDate) : IRequest<Result<OccupancyReportDto>>;

public sealed class GetOccupancyReportQueryHandler : IRequestHandler<GetOccupancyReportQuery, Result<OccupancyReportDto>>
{
    private readonly IReportService _reportService;

    public GetOccupancyReportQueryHandler(IReportService reportService)
    {
        _reportService = reportService;
    }

    public Task<Result<OccupancyReportDto>> Handle(GetOccupancyReportQuery request, CancellationToken cancellationToken)
        => _reportService.GetOccupancyAsync(request.StartDate, request.EndDate, cancellationToken);
}

public sealed record GetMonthlyRevenueReportQuery(DateOnly StartDate, DateOnly EndDate) : IRequest<Result<IReadOnlyList<MonthlyRevenueDto>>>;

public sealed class GetMonthlyRevenueReportQueryHandler : IRequestHandler<GetMonthlyRevenueReportQuery, Result<IReadOnlyList<MonthlyRevenueDto>>>
{
    private readonly IReportService _reportService;

    public GetMonthlyRevenueReportQueryHandler(IReportService reportService)
    {
        _reportService = reportService;
    }

    public Task<Result<IReadOnlyList<MonthlyRevenueDto>>> Handle(GetMonthlyRevenueReportQuery request, CancellationToken cancellationToken)
        => _reportService.GetMonthlyRevenueAsync(request.StartDate, request.EndDate, cancellationToken);
}

public sealed record GetPopularRoomTypesReportQuery(DateOnly StartDate, DateOnly EndDate) : IRequest<Result<IReadOnlyList<PopularRoomTypeDto>>>;

public sealed class GetPopularRoomTypesReportQueryHandler : IRequestHandler<GetPopularRoomTypesReportQuery, Result<IReadOnlyList<PopularRoomTypeDto>>>
{
    private readonly IReportService _reportService;

    public GetPopularRoomTypesReportQueryHandler(IReportService reportService)
    {
        _reportService = reportService;
    }

    public Task<Result<IReadOnlyList<PopularRoomTypeDto>>> Handle(GetPopularRoomTypesReportQuery request, CancellationToken cancellationToken)
        => _reportService.GetPopularRoomTypesAsync(request.StartDate, request.EndDate, cancellationToken);
}
