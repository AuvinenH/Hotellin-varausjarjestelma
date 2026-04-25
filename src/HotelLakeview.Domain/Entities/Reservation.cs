using HotelLakeview.Domain.Enums;
using HotelLakeview.Domain.ValueObjects;

namespace HotelLakeview.Domain.Entities;

public class Reservation
{
    public Reservation(Guid id, Guid roomId, Guid customerId, DateRange stayPeriod, int guestCount, decimal totalPrice)
    {
        if (guestCount < 1)
        {
            throw new ArgumentException("Guest count must be greater than zero.");
        }

        if (totalPrice <= 0)
        {
            throw new ArgumentException("Total price must be greater than zero.");
        }

        Id = id;
        RoomId = roomId;
        CustomerId = customerId;
        CheckInDate = stayPeriod.CheckInDate;
        CheckOutDate = stayPeriod.CheckOutDate;
        GuestCount = guestCount;
        TotalPrice = decimal.Round(totalPrice, 2, MidpointRounding.AwayFromZero);
        Status = ReservationStatus.Confirmed;
        CreatedAtUtc = DateTime.UtcNow;
    }

    private Reservation()
    {
    }

    public Guid Id { get; private set; }

    public Guid RoomId { get; private set; }

    public Guid CustomerId { get; private set; }

    public DateOnly CheckInDate { get; private set; }

    public DateOnly CheckOutDate { get; private set; }

    public int GuestCount { get; private set; }

    public decimal TotalPrice { get; private set; }

    public ReservationStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? CancelledAtUtc { get; private set; }

    public DateRange StayPeriod => new(CheckInDate, CheckOutDate);

    public void Update(Guid roomId, DateRange stayPeriod, int guestCount, decimal totalPrice)
    {
        if (Status == ReservationStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled reservation cannot be modified.");
        }

        if (guestCount < 1)
        {
            throw new ArgumentException("Guest count must be greater than zero.");
        }

        if (totalPrice <= 0)
        {
            throw new ArgumentException("Total price must be greater than zero.");
        }

        RoomId = roomId;
        CheckInDate = stayPeriod.CheckInDate;
        CheckOutDate = stayPeriod.CheckOutDate;
        GuestCount = guestCount;
        TotalPrice = decimal.Round(totalPrice, 2, MidpointRounding.AwayFromZero);
    }

    public void Cancel()
    {
        if (Status == ReservationStatus.Cancelled)
        {
            return;
        }

        Status = ReservationStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
    }
}
