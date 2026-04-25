using HotelLakeview.Domain.Entities;

namespace HotelLakeview.Application.Abstractions;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(string? search, CancellationToken cancellationToken);

    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Customer?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

    Task AddAsync(Customer customer, CancellationToken cancellationToken);

    void Remove(Customer customer);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
