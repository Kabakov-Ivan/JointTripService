using JointTripService.Application.Models;
using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Interfaces;
using JointTripService.ValueObjects;

namespace JointTripService.Application.Services;

public class BookingApplicationService(IBookingRepository repository, IPassengerRepository passengerRepository, ITripRepository tripRepository, IDriverRepository driverRepository)
{
    public async Task<IEnumerable<BookingModel>> GetAllAsync(CancellationToken ct) => (await repository.GetAllAsync(ct, true)).Select(ModelMapper.Map);

    public async Task<BookingModel?> GetAsync(Guid id, CancellationToken ct)
        => (await repository.GetByIdAsync(id, ct)) is { } x ? ModelMapper.Map(x) : null;

    public async Task<BookingModel?> CreateAsync(CreateBookingModel model, CancellationToken ct)
    {
        var passenger = await passengerRepository.GetByIdAsync(model.PassengerId, ct);
        var trip = await tripRepository.GetByIdAsync(model.TripId, ct);
        if (passenger is null || trip is null)
            return null;

        var entity = passenger.BookTrip(trip, new SeatsCount(model.SeatsCount));
        return (await repository.AddAsync(entity, ct)) is { } created ? ModelMapper.Map(created) : null;
    }

    public async Task<bool> ApproveAsync(Guid id, CancellationToken ct)
        => await ChangeAsync(id, ct, (driver, booking) => driver.ApproveBooking(booking));

    public async Task<bool> RejectAsync(Guid id, CancellationToken ct)
        => await ChangeAsync(id, ct, (driver, booking) => driver.RejectBooking(booking));

    public async Task<bool> CancelAsync(Guid id, CancellationToken ct)
    {
        var booking = await repository.GetByIdAsync(id, ct);
        return booking is not null && booking.Cancel() && await repository.UpdateAsync(booking, ct);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => repository.DeleteAsync(id, ct);

    private async Task<bool> ChangeAsync(Guid id, CancellationToken ct, Func<Driver, Booking, bool> action)
    {
        var booking = await repository.GetByIdAsync(id, ct);
        if (booking is null)
            return false;

        var driver = await driverRepository.GetByIdAsync(booking.Trip.Driver.Id, ct);
        return driver is not null && action(driver, booking) && await repository.UpdateAsync(booking, ct);
    }
}
