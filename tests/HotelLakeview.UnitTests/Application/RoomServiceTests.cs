using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.Contracts.Rooms;
using HotelLakeview.Application.Services;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using HotelLakeview.Domain.ValueObjects;
using Moq;

namespace HotelLakeview.UnitTests.Application;

public class RoomServiceTests
{
    [Fact]
    public async Task CreateAsync_AddsAndSaves_WhenRoomNumberIsUnique()
    {
        var repository = new Mock<IRoomRepository>();
        var service = new RoomService(repository.Object);
        var request = new CreateRoomRequest(" 101 ", RoomCategory.Standard, 2, 120m, " Nice room ");

        repository
            .Setup(r => r.GetByNumberAsync("101", CancellationToken.None))
            .ReturnsAsync((Room?)null);

        var result = await service.CreateAsync(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("101", result.Value.Number);
        Assert.Equal(RoomCategory.Standard, result.Value.Category);

        repository.Verify(r => r.AddAsync(It.IsAny<Room>(), CancellationToken.None), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsConflict_WhenRoomNumberAlreadyExists()
    {
        var repository = new Mock<IRoomRepository>();
        var service = new RoomService(repository.Object);
        var request = new CreateRoomRequest("201", RoomCategory.Economy, 1, 80m, null);

        repository
            .Setup(r => r.GetByNumberAsync("201", CancellationToken.None))
            .ReturnsAsync(new Room(Guid.NewGuid(), "201", RoomCategory.Economy, 1, 80m, null));

        var result = await service.CreateAsync(request, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ResultErrorType.Conflict, result.Error!.Type);

        repository.Verify(r => r.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsConflict_WhenAnotherRoomUsesSameNumber()
    {
        var repository = new Mock<IRoomRepository>();
        var service = new RoomService(repository.Object);

        var targetRoom = new Room(Guid.NewGuid(), "301", RoomCategory.Standard, 2, 100m, null);
        var otherRoom = new Room(Guid.NewGuid(), "302", RoomCategory.Standard, 2, 100m, null);
        var request = new UpdateRoomRequest("302", RoomCategory.Superior, 2, 140m, "Updated");

        repository.Setup(r => r.GetByIdAsync(targetRoom.Id, CancellationToken.None)).ReturnsAsync(targetRoom);
        repository.Setup(r => r.GetByNumberAsync("302", CancellationToken.None)).ReturnsAsync(otherRoom);

        var result = await service.UpdateAsync(targetRoom.Id, request, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ResultErrorType.Conflict, result.Error!.Type);

        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAvailableAsync_PassesDateRangeGuestCountAndCategoryToRepository()
    {
        var repository = new Mock<IRoomRepository>();
        var service = new RoomService(repository.Object);

        var checkInDate = new DateOnly(2026, 8, 1);
        var checkOutDate = new DateOnly(2026, 8, 5);
        const int guestCount = 2;
        var category = RoomCategory.Superior;

        repository.Setup(r => r.GetAvailableAsync(
                It.Is<DateRange>(range => range.CheckInDate == checkInDate && range.CheckOutDate == checkOutDate),
                guestCount,
                category,
                CancellationToken.None))
            .ReturnsAsync(new List<Room>
            {
                new(Guid.NewGuid(), "401", RoomCategory.Superior, 2, 170m, "Sea view"),
            });

        var result = await service.GetAvailableAsync(checkInDate, checkOutDate, guestCount, category, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("401", result.Value[0].Number);

        repository.Verify(r => r.GetAvailableAsync(
            It.Is<DateRange>(range => range.CheckInDate == checkInDate && range.CheckOutDate == checkOutDate),
            guestCount,
            category,
            CancellationToken.None), Times.Once);
    }
}
