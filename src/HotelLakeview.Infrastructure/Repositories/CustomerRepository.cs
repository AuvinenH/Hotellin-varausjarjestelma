using HotelLakeview.Application.Abstractions;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelLakeview.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly HotelDbContext _dbContext;

    public CustomerRepository(HotelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(string? search, CancellationToken cancellationToken)
    {
        var query = _dbContext.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLowerInvariant();

            query = query.Where(customer =>
                customer.FullName.ToLower().Contains(normalizedSearch)
                || customer.Email.ToLower().Contains(normalizedSearch)
                || customer.PhoneNumber.ToLower().Contains(normalizedSearch));
        }

        return await query.OrderBy(customer => customer.FullName).ToListAsync(cancellationToken);
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Customers.FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);
    }

    public Task<Customer?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return _dbContext.Customers.FirstOrDefaultAsync(
            customer => customer.Email == normalizedEmail,
            cancellationToken);
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        return _dbContext.Customers.AddAsync(customer, cancellationToken).AsTask();
    }

    public void Remove(Customer customer)
    {
        _dbContext.Customers.Remove(customer);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
