namespace HotelLakeview.Domain.Entities;

public class Customer
{
    public Customer(Guid id, string fullName, string email, string phoneNumber, string? notes)
    {
        Id = id;
        UpdateDetails(fullName, email, phoneNumber, notes);
        CreatedAtUtc = DateTime.UtcNow;
    }

    private Customer()
    {
    }

    public Guid Id { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PhoneNumber { get; private set; } = string.Empty;

    public string? Notes { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public void UpdateDetails(string fullName, string email, string phoneNumber, string? notes)
    {
        FullName = string.IsNullOrWhiteSpace(fullName)
            ? throw new ArgumentException("Full name is required.")
            : fullName.Trim();

        Email = string.IsNullOrWhiteSpace(email)
            ? throw new ArgumentException("Email is required.")
            : email.Trim().ToLowerInvariant();

        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber)
            ? throw new ArgumentException("Phone number is required.")
            : phoneNumber.Trim();

        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }
}
