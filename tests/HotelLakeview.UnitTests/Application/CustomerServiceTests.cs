using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Customers;
using HotelLakeview.Application.Exceptions;
using HotelLakeview.Application.Services;
using HotelLakeview.Domain.Entities;
using Moq;

namespace HotelLakeview.UnitTests.Application;

public class CustomerServiceTests
{
    [Fact]
    public async Task CreateAsync_AddsAndSaves_WhenEmailIsUnique()
    {
        var repository = new Mock<ICustomerRepository>();
        var service = new CustomerService(repository.Object);
        var request = new CreateCustomerRequest(" Test User ", "TEST@EXAMPLE.COM", "040 1234567", " note ");

        repository
            .Setup(r => r.GetByEmailAsync("test@example.com", CancellationToken.None))
            .ReturnsAsync((Customer?)null);

        var result = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal("Test User", result.FullName);
        Assert.Equal("test@example.com", result.Email);

        repository.Verify(r => r.AddAsync(It.IsAny<Customer>(), CancellationToken.None), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ThrowsConflict_WhenEmailAlreadyExists()
    {
        var repository = new Mock<ICustomerRepository>();
        var service = new CustomerService(repository.Object);
        var request = new CreateCustomerRequest("User", "dup@example.com", "040", null);

        repository
            .Setup(r => r.GetByEmailAsync("dup@example.com", CancellationToken.None))
            .ReturnsAsync(new Customer(Guid.NewGuid(), "Existing", "dup@example.com", "123", null));

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(request, CancellationToken.None));

        repository.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFound_WhenCustomerMissing()
    {
        var repository = new Mock<ICustomerRepository>();
        var service = new CustomerService(repository.Object);
        var id = Guid.NewGuid();

        repository.Setup(r => r.GetByIdAsync(id, CancellationToken.None)).ReturnsAsync((Customer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(id, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_RemovesAndSaves_WhenCustomerExists()
    {
        var repository = new Mock<ICustomerRepository>();
        var service = new CustomerService(repository.Object);
        var customer = new Customer(Guid.NewGuid(), "Delete User", "delete@example.com", "040", null);

        repository.Setup(r => r.GetByIdAsync(customer.Id, CancellationToken.None)).ReturnsAsync(customer);

        await service.DeleteAsync(customer.Id, CancellationToken.None);

        repository.Verify(r => r.Remove(customer), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}
