using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Customers;
using HotelLakeview.Application.Exceptions;
using HotelLakeview.Domain.Entities;

namespace HotelLakeview.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(string? search, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(search, cancellationToken);
        return customers.Select(Map).ToList();
    }

    public async Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Customer '{id}' was not found.");

        return Map(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existing = await _customerRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (existing is not null)
        {
            throw new ConflictException("Customer with the same email already exists.");
        }

        var customer = new Customer(Guid.NewGuid(), request.FullName, request.Email, request.PhoneNumber, request.Notes);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return Map(customer);
    }

    public async Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Customer '{id}' was not found.");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existing = await _customerRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (existing is not null && existing.Id != id)
        {
            throw new ConflictException("Customer with the same email already exists.");
        }

        customer.UpdateDetails(request.FullName, request.Email, request.PhoneNumber, request.Notes);

        await _customerRepository.SaveChangesAsync(cancellationToken);
        return Map(customer);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Customer '{id}' was not found.");

        _customerRepository.Remove(customer);
        await _customerRepository.SaveChangesAsync(cancellationToken);
    }

    private static CustomerDto Map(Customer customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.PhoneNumber,
            customer.Notes,
            customer.CreatedAtUtc);
    }
}
