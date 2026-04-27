using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.Services;
using HotelLakeview.Domain.Enums;
using HotelLakeview.Domain.ValueObjects;
using Moq;

namespace HotelLakeview.UnitTests.Application;

public class ReportServiceTests
{
    [Fact]
    public async Task GetOccupancyAsync_ReturnsExpectedMetrics()
    {
        var reservationRepository = new Mock<IReservationRepository>();
        var roomRepository = new Mock<IRoomRepository>();
        var service = new ReportService(reservationRepository.Object, roomRepository.Object);

        var startDate = new DateOnly(2026, 1, 1);
        var endDate = new DateOnly(2026, 1, 6);

        roomRepository.Setup(repository => repository.CountAsync(CancellationToken.None))
            .ReturnsAsync(10);

        reservationRepository
            .Setup(repository => repository.CountBookedNightsAsync(
                It.Is<DateRange>(range => range.CheckInDate == startDate && range.CheckOutDate == endDate),
                CancellationToken.None))
            .ReturnsAsync(25);

        var result = await service.GetOccupancyAsync(startDate, endDate, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(startDate, result.Value.StartDate);
        Assert.Equal(endDate, result.Value.EndDate);
        Assert.Equal(25, result.Value.ReservedNights);
        Assert.Equal(50, result.Value.TotalRoomNights);
        Assert.Equal(50m, result.Value.OccupancyRatePercent);
    }

    [Fact]
    public async Task GetOccupancyAsync_UsesRepositoryDependenciesOnce()
    {
        var reservationRepository = new Mock<IReservationRepository>();
        var roomRepository = new Mock<IRoomRepository>();
        var service = new ReportService(reservationRepository.Object, roomRepository.Object);

        var startDate = new DateOnly(2026, 2, 1);
        var endDate = new DateOnly(2026, 2, 3);

        roomRepository.Setup(repository => repository.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(4);
        reservationRepository.Setup(repository => repository.CountBookedNightsAsync(It.IsAny<DateRange>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        await service.GetOccupancyAsync(startDate, endDate, CancellationToken.None);

        roomRepository.Verify(repository => repository.CountAsync(CancellationToken.None), Times.Once);
        reservationRepository.Verify(repository => repository.CountBookedNightsAsync(
            It.Is<DateRange>(range => range.CheckInDate == startDate && range.CheckOutDate == endDate),
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetMonthlyRevenueAsync_ReturnsDataOrderedByYearAndMonth()
    {
        var reservationRepository = new Mock<IReservationRepository>();
        var roomRepository = new Mock<IRoomRepository>();
        var service = new ReportService(reservationRepository.Object, roomRepository.Object);

        var startDate = new DateOnly(2026, 1, 1);
        var endDate = new DateOnly(2026, 12, 31);

        reservationRepository.Setup(repository => repository.GetMonthlyRevenueAsync(startDate, endDate, CancellationToken.None))
            .ReturnsAsync(new List<(int Year, int Month, decimal Revenue)>
            {
                (2026, 3, 1200m),
                (2025, 12, 800m),
                (2026, 1, 900m),
            });

        var result = await service.GetMonthlyRevenueAsync(startDate, endDate, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Count);
        Assert.Equal((2025, 12, 800m), (result.Value[0].Year, result.Value[0].Month, result.Value[0].Revenue));
        Assert.Equal((2026, 1, 900m), (result.Value[1].Year, result.Value[1].Month, result.Value[1].Revenue));
        Assert.Equal((2026, 3, 1200m), (result.Value[2].Year, result.Value[2].Month, result.Value[2].Revenue));
    }

    [Fact]
    public async Task GetPopularRoomTypesAsync_ReturnsDataOrderedByNightCountDescending()
    {
        var reservationRepository = new Mock<IReservationRepository>();
        var roomRepository = new Mock<IRoomRepository>();
        var service = new ReportService(reservationRepository.Object, roomRepository.Object);

        var startDate = new DateOnly(2026, 1, 1);
        var endDate = new DateOnly(2026, 12, 31);

        reservationRepository.Setup(repository => repository.GetPopularRoomTypesAsync(startDate, endDate, CancellationToken.None))
            .ReturnsAsync(new List<(RoomCategory Category, int ReservationCount, int NightCount)>
            {
                (RoomCategory.Standard, 8, 22),
                (RoomCategory.Suite, 2, 10),
                (RoomCategory.Economy, 12, 30),
            });

        var result = await service.GetPopularRoomTypesAsync(startDate, endDate, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Count);
        Assert.Equal(RoomCategory.Economy, result.Value[0].Category);
        Assert.Equal(RoomCategory.Standard, result.Value[1].Category);
        Assert.Equal(RoomCategory.Suite, result.Value[2].Category);
    }

    [Fact]
    public async Task GetMonthlyRevenueAsync_ReturnsValidationFailure_WhenDateRangeIsInvalid()
    {
        var reservationRepository = new Mock<IReservationRepository>();
        var roomRepository = new Mock<IRoomRepository>();
        var service = new ReportService(reservationRepository.Object, roomRepository.Object);

        var startDate = new DateOnly(2026, 5, 10);
        var endDate = new DateOnly(2026, 5, 9);

        var result = await service.GetMonthlyRevenueAsync(startDate, endDate, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ResultErrorType.Validation, result.Error!.Type);

        reservationRepository.Verify(repository => repository.GetMonthlyRevenueAsync(
            It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetPopularRoomTypesAsync_ReturnsValidationFailure_WhenDateRangeIsInvalid()
    {
        var reservationRepository = new Mock<IReservationRepository>();
        var roomRepository = new Mock<IRoomRepository>();
        var service = new ReportService(reservationRepository.Object, roomRepository.Object);

        var startDate = new DateOnly(2026, 7, 10);
        var endDate = new DateOnly(2026, 7, 10);

        var result = await service.GetPopularRoomTypesAsync(startDate, endDate, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ResultErrorType.Validation, result.Error!.Type);

        reservationRepository.Verify(repository => repository.GetPopularRoomTypesAsync(
            It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}
