using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Reports;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("occupancy")]
    public async Task<ActionResult<OccupancyReportDto>> GetOccupancy(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var report = await _reportService.GetOccupancyAsync(startDate, endDate, cancellationToken);
        return Ok(report);
    }

    [HttpGet("monthly-revenue")]
    public async Task<ActionResult<IReadOnlyList<MonthlyRevenueDto>>> GetMonthlyRevenue(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var report = await _reportService.GetMonthlyRevenueAsync(startDate, endDate, cancellationToken);
        return Ok(report);
    }

    [HttpGet("popular-room-types")]
    public async Task<ActionResult<IReadOnlyList<PopularRoomTypeDto>>> GetPopularRoomTypes(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var report = await _reportService.GetPopularRoomTypesAsync(startDate, endDate, cancellationToken);
        return Ok(report);
    }
}
