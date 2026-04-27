using HotelLakeview.Application.Contracts.Reservations;
using HotelLakeview.Application.Common;

namespace HotelLakeview.Application.Abstractions;

public interface IReservationService
{
    Task<Result<IReadOnlyList<ReservationDto>>> GetAllAsync(CancellationToken cancellationToken);

    Task<Result<ReservationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<ReservationDto>> CreateAsync(CreateReservationRequest request, CancellationToken cancellationToken);

    Task<Result<ReservationDto>> UpdateAsync(Guid id, UpdateReservationRequest request, CancellationToken cancellationToken);

    Task<Result> CancelAsync(Guid id, CancellationToken cancellationToken);
}
