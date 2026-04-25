namespace HotelLakeview.Application.Contracts.Customers;

public sealed record CreateCustomerRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    string? Notes);
