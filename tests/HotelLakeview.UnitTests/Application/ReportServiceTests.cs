using HotelLakeview.Application.Abstractions;
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

        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(endDate, result.EndDate);
        Assert.Equal(25, result.ReservedNights);
        Assert.Equal(50, result.TotalRoomNights);
        Assert.Equal(50m, result.OccupancyRatePercent);
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

        Assert.Equal(3, result.Count);
        Assert.Equal((2025, 12, 800m), (result[0].Year, result[0].Month, result[0].Revenue));
        Assert.Equal((2026, 1, 900m), (result[1].Year, result[1].Month, result[1].Revenue));
        Assert.Equal((2026, 3, 1200m), (result[2].Year, result[2].Month, result[2].Revenue));
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

        Assert.Equal(3, result.Count);
        Assert.Equal(RoomCategory.Economy, result[0].Category);
        Assert.Equal(RoomCategory.Standard, result[1].Category);
        Assert.Equal(RoomCategory.Suite, result[2].Category);
    }

    [Fact]
    public async Task GetMonthlyRevenueAsync_Throws_WhenDateRangeIsInvalid()
    {
        var reservationRepository = new Mock<IReservationRepository>();
        var roomRepository = new Mock<IRoomRepository>();
        var service = new ReportService(reservationRepository.Object, roomRepository.Object);

        var startDate = new DateOnly(2026, 5, 10);
        var endDate = new DateOnly(2026, 5, 9);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetMonthlyRevenueAsync(startDate, endDate, CancellationToken.None));

        reservationRepository.Verify(repository => repository.GetMonthlyRevenueAsync(
            It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetPopularRoomTypesAsync_Throws_WhenDateRangeIsInvalid()
    {
        var reservationRepository = new Mock<IReservationRepository>();
        var roomRepository = new Mock<IRoomRepository>();
        var service = new ReportService(reservationRepository.Object, roomRepository.Object);

        var startDate = new DateOnly(2026, 7, 10);
        var endDate = new DateOnly(2026, 7, 10);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetPopularRoomTypesAsync(startDate, endDate, CancellationToken.None));

        reservationRepository.Verify(repository => repository.GetPopularRoomTypesAsync(
            It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}
