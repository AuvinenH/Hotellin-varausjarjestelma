namespace HotelLakeview.Domain.ValueObjects;

public sealed record DateRange
{
    public DateRange(DateOnly checkInDate, DateOnly checkOutDate)
    {
        if (checkOutDate <= checkInDate)
        {
            throw new ArgumentException("Check-out date must be after check-in date.");
        }

        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
    }

    public DateOnly CheckInDate { get; }

    public DateOnly CheckOutDate { get; }

    public int Nights => CheckOutDate.DayNumber - CheckInDate.DayNumber;

    public IEnumerable<DateOnly> EnumerateNights()
    {
        for (var date = CheckInDate; date < CheckOutDate; date = date.AddDays(1))
        {
            yield return date;
        }
    }

    public bool Overlaps(DateRange other)
    {
        return CheckInDate < other.CheckOutDate && CheckOutDate > other.CheckInDate;
    }
}
