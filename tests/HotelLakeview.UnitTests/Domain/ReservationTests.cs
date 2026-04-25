using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using HotelLakeview.Domain.ValueObjects;

namespace HotelLakeview.UnitTests.Domain;

public class ReservationTests
{
    [Fact]
    public void Constructor_SetsConfirmedStatus()
    {
        var reservation = new Reservation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateRange(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 3)),
            2,
            260m);

        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
    }

    [Fact]
    public void Cancel_SetsStatusToCancelled()
    {
        var reservation = new Reservation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateRange(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 3)),
            2,
            260m);

        reservation.Cancel();

        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
        Assert.NotNull(reservation.CancelledAtUtc);
    }

    [Fact]
    public void Update_Throws_WhenReservationIsCancelled()
    {
        var reservation = new Reservation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateRange(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 3)),
            2,
            260m);

        reservation.Cancel();

        Assert.Throws<InvalidOperationException>(() => reservation.Update(
            Guid.NewGuid(),
            new DateRange(new DateOnly(2026, 7, 2), new DateOnly(2026, 7, 5)),
            2,
            390m));
    }

    [Fact]
    public void Update_ChangesStayPeriodAndPrice()
    {
        var roomId = Guid.NewGuid();
        var reservation = new Reservation(
            Guid.NewGuid(),
            roomId,
            Guid.NewGuid(),
            new DateRange(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 3)),
            2,
            260m);

        reservation.Update(
            roomId,
            new DateRange(new DateOnly(2026, 7, 2), new DateOnly(2026, 7, 6)),
            2,
            520m);

        Assert.Equal(new DateOnly(2026, 7, 2), reservation.CheckInDate);
        Assert.Equal(new DateOnly(2026, 7, 6), reservation.CheckOutDate);
        Assert.Equal(520m, reservation.TotalPrice);
    }
}
