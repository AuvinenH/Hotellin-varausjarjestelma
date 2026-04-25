using HotelLakeview.Application.Contracts.Reservations;

namespace HotelLakeview.Application.Abstractions;

public interface IReservationService
{
    Task<IReadOnlyList<ReservationDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<ReservationDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ReservationDto> CreateAsync(CreateReservationRequest request, CancellationToken cancellationToken);

    Task<ReservationDto> UpdateAsync(Guid id, UpdateReservationRequest request, CancellationToken cancellationToken);

    Task CancelAsync(Guid id, CancellationToken cancellationToken);
}
