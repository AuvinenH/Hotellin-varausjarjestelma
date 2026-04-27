using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Customers;
using HotelLakeview.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var result = await _customerService.GetAllAsync(search, cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _customerService.GetByIdAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await _customerService.CreateAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await _customerService.UpdateAsync(id, request, cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _customerService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : this.ToProblem(result.Error!);
    }
}
