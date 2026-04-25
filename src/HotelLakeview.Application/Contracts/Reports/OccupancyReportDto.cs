namespace HotelLakeview.Application.Contracts.Reports;

public sealed record OccupancyReportDto(
    DateOnly StartDate,
    DateOnly EndDate,
    int ReservedNights,
    int TotalRoomNights,
    decimal OccupancyRatePercent);
