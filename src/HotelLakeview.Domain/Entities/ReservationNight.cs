namespace HotelLakeview.Domain.Entities;

public class ReservationNight
{
    public ReservationNight(Guid reservationId, Guid roomId, DateOnly nightDate)
    {
        ReservationId = reservationId;
        RoomId = roomId;
        NightDate = nightDate;
    }

    private ReservationNight()
    {
    }

    public Guid ReservationId { get; private set; }

    public Guid RoomId { get; private set; }

    public DateOnly NightDate { get; private set; }

    public Reservation? Reservation { get; private set; }
}
