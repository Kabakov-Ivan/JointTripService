using JointTripService.Application.Models;
using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Interfaces;
using JointTripService.ValueObjects;

namespace JointTripService.Application.Services;

public class TripApplicationService(ITripRepository repository, IDriverRepository driverRepository)
{
    public async Task<IEnumerable<TripModel>> GetAllAsync(CancellationToken ct) => (await repository.GetAllAsync(ct, true)).Select(ModelMapper.Map);

    public async Task<IEnumerable<TripModel>> GetPublishedAsync(CancellationToken ct) => (await repository.GetPublishedTripsAsync(ct, true)).Select(ModelMapper.Map);

    public async Task<TripModel?> GetAsync(Guid id, CancellationToken ct)
        => (await repository.GetByIdAsync(id, ct)) is { } x ? ModelMapper.Map(x) : null;

    public async Task<TripModel?> CreateAsync(CreateTripModel model, CancellationToken ct)
    {
        var driver = await driverRepository.GetByIdAsync(model.DriverId, ct);
        if (driver is null)
            return null;

        var entity = driver.CreateTrip(new City(model.Origin), new City(model.Destination), model.DepartureAt, new SeatsCount(model.SeatsCount), model.Description);
        return (await repository.AddAsync(entity, ct)) is { } created ? ModelMapper.Map(created) : null;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTripModel model, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null)
            return false;

        var driver = await driverRepository.GetByIdAsync(entity.Driver.Id, ct);
        if (driver is null)
            return false;

        var changed = driver.EditTrip(entity, new City(model.Origin), new City(model.Destination), model.DepartureAt, new SeatsCount(model.SeatsCount), model.Description);
        return changed && await repository.UpdateAsync(entity, ct);
    }

    public async Task<bool> PublishAsync(Guid id, CancellationToken ct)
        => await ChangeAsync(id, ct, (driver, trip) => driver.PublishTrip(trip));

    public async Task<bool> CancelAsync(Guid id, CancellationToken ct)
        => await ChangeAsync(id, ct, (driver, trip) => driver.CancelTrip(trip));

    public async Task<bool> CompleteAsync(Guid id, CancellationToken ct)
        => await ChangeAsync(id, ct, (driver, trip) => driver.CompleteTrip(trip));

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => repository.DeleteAsync(id, ct);

    private async Task<bool> ChangeAsync(Guid id, CancellationToken ct, Func<Driver, Trip, bool> action)
    {
        var trip = await repository.GetByIdAsync(id, ct);
        if (trip is null)
            return false;

        var driver = await driverRepository.GetByIdAsync(trip.Driver.Id, ct);
        return driver is not null && action(driver, trip) && await repository.UpdateAsync(trip, ct);
    }
}
