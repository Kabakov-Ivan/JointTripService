using JointTripService.Application.Models;
using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Base;
using JointTripService.Domain.Repositories.Abstractions.Interfaces;
using JointTripService.ValueObjects;

namespace JointTripService.Application.Services;

public static class ModelMapper
{
    public static DriverModel Map(Driver x) => new(x.Id, x.FullName.Value, x.Email.Value, x.CreationData, x.ModificationData);
    public static PassengerModel Map(Passenger x) => new(x.Id, x.FullName.Value, x.Email.Value, x.CreationData, x.ModificationData);
    public static TripModel Map(Trip x) => new(x.Id, x.Driver.Id, x.Origin.Value, x.Destination.Value, x.DepartureAt, x.SeatsCount.Value, x.AvailableSeats, x.Description, x.Status.ToString(), x.CreationData, x.ModificationData);
    public static BookingModel Map(Booking x) => new(x.Id, x.Passenger.Id, x.Trip.Id, x.SeatsCount.Value, x.Status.ToString(), x.CreationData, x.ModificationData);
    public static ReviewModel Map(Review x) => new(x.Id, x.AuthorId, x.TargetId, x.AuthorIsDriver, x.TargetIsDriver, x.Trip.Id, x.Rating, x.Text.Value, x.CreationData, x.ModificationData);
}

public class DriverApplicationService(IDriverRepository repository) 
{
    public async Task<IEnumerable<DriverModel>> GetAllAsync(CancellationToken ct) => (await repository.GetAllAsync(ct, true)).Select(ModelMapper.Map);
    public async Task<DriverModel?> GetAsync(Guid id, CancellationToken ct) => (await repository.GetByIdAsync(id, ct)) is { } x ? ModelMapper.Map(x) : null;
    public async Task<DriverModel?> CreateAsync(CreateDriverModel model, CancellationToken ct)
    {
        if (await repository.GetDriverByEmailAsync(model.Email, ct) is not null) return null;
        var entity = new Driver(Guid.NewGuid(), new FullName(model.FullName), new Email(model.Email));
        return (await repository.AddAsync(entity, ct)) is { } created ? ModelMapper.Map(created) : null;
    }
    public async Task<bool> UpdateAsync(Guid id, UpdateDriverModel model, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        var changed = entity.ChangeFullName(new FullName(model.FullName)) | entity.ChangeEmail(new Email(model.Email));
        return changed && await repository.UpdateAsync(entity, ct);
    }
    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => repository.DeleteAsync(id, ct);
}

public class PassengerApplicationService(IPassengerRepository repository)
{
    public async Task<IEnumerable<PassengerModel>> GetAllAsync(CancellationToken ct) => (await repository.GetAllAsync(ct, true)).Select(ModelMapper.Map);
    public async Task<PassengerModel?> GetAsync(Guid id, CancellationToken ct) => (await repository.GetByIdAsync(id, ct)) is { } x ? ModelMapper.Map(x) : null;
    public async Task<PassengerModel?> CreateAsync(CreatePassengerModel model, CancellationToken ct)
    {
        if (await repository.GetPassengerByEmailAsync(model.Email, ct) is not null) return null;
        var entity = new Passenger(Guid.NewGuid(), new FullName(model.FullName), new Email(model.Email));
        return (await repository.AddAsync(entity, ct)) is { } created ? ModelMapper.Map(created) : null;
    }
    public async Task<bool> UpdateAsync(Guid id, UpdatePassengerModel model, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        var changed = entity.ChangeFullName(new FullName(model.FullName)) | entity.ChangeEmail(new Email(model.Email));
        return changed && await repository.UpdateAsync(entity, ct);
    }
    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => repository.DeleteAsync(id, ct);
}

public class TripApplicationService(ITripRepository repository, IDriverRepository driverRepository)
{
    public async Task<IEnumerable<TripModel>> GetAllAsync(CancellationToken ct) => (await repository.GetAllAsync(ct, true)).Select(ModelMapper.Map);
    public async Task<IEnumerable<TripModel>> GetPublishedAsync(CancellationToken ct) => (await repository.GetPublishedTripsAsync(ct, true)).Select(ModelMapper.Map);
    public async Task<TripModel?> GetAsync(Guid id, CancellationToken ct) => (await repository.GetByIdAsync(id, ct)) is { } x ? ModelMapper.Map(x) : null;
    public async Task<TripModel?> CreateAsync(CreateTripModel model, CancellationToken ct)
    {
        var driver = await driverRepository.GetByIdAsync(model.DriverId, ct);
        if (driver is null) return null;
        var entity = driver.CreateTrip(new City(model.Origin), new City(model.Destination), model.DepartureAt, new SeatsCount(model.SeatsCount), model.Description);
        return (await repository.AddAsync(entity, ct)) is { } created ? ModelMapper.Map(created) : null;
    }
    public async Task<bool> UpdateAsync(Guid id, UpdateTripModel model, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        var driver = await driverRepository.GetByIdAsync(entity.Driver.Id, ct);
        if (driver is null) return false;
        var changed = driver.EditTrip(entity, new City(model.Origin), new City(model.Destination), model.DepartureAt, new SeatsCount(model.SeatsCount), model.Description);
        return changed && await repository.UpdateAsync(entity, ct);
    }
    public async Task<bool> PublishAsync(Guid id, CancellationToken ct) => await ChangeAsync(id, ct, (driver, trip) => driver.PublishTrip(trip));
    public async Task<bool> CancelAsync(Guid id, CancellationToken ct) => await ChangeAsync(id, ct, (driver, trip) => driver.CancelTrip(trip));
    public async Task<bool> CompleteAsync(Guid id, CancellationToken ct) => await ChangeAsync(id, ct, (driver, trip) => driver.CompleteTrip(trip));
    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => repository.DeleteAsync(id, ct);
    private async Task<bool> ChangeAsync(Guid id, CancellationToken ct, Func<Driver, Trip, bool> action)
    {
        var trip = await repository.GetByIdAsync(id, ct);
        if (trip is null) return false;
        var driver = await driverRepository.GetByIdAsync(trip.Driver.Id, ct);
        return driver is not null && action(driver, trip) && await repository.UpdateAsync(trip, ct);
    }
}

public class BookingApplicationService(IBookingRepository repository, IPassengerRepository passengerRepository, ITripRepository tripRepository, IDriverRepository driverRepository)
{
    public async Task<IEnumerable<BookingModel>> GetAllAsync(CancellationToken ct) => (await repository.GetAllAsync(ct, true)).Select(ModelMapper.Map);
    public async Task<BookingModel?> GetAsync(Guid id, CancellationToken ct) => (await repository.GetByIdAsync(id, ct)) is { } x ? ModelMapper.Map(x) : null;
    public async Task<BookingModel?> CreateAsync(CreateBookingModel model, CancellationToken ct)
    {
        var passenger = await passengerRepository.GetByIdAsync(model.PassengerId, ct);
        var trip = await tripRepository.GetByIdAsync(model.TripId, ct);
        if (passenger is null || trip is null) return null;
        var entity = passenger.BookTrip(trip, new SeatsCount(model.SeatsCount));
        return (await repository.AddAsync(entity, ct)) is { } created ? ModelMapper.Map(created) : null;
    }
    public async Task<bool> ApproveAsync(Guid id, CancellationToken ct) => await ChangeAsync(id, ct, (driver, booking) => driver.ApproveBooking(booking));
    public async Task<bool> RejectAsync(Guid id, CancellationToken ct) => await ChangeAsync(id, ct, (driver, booking) => driver.RejectBooking(booking));
    public async Task<bool> CancelAsync(Guid id, CancellationToken ct)
    {
        var booking = await repository.GetByIdAsync(id, ct);
        return booking is not null && booking.Cancel() && await repository.UpdateAsync(booking, ct);
    }
    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => repository.DeleteAsync(id, ct);
    private async Task<bool> ChangeAsync(Guid id, CancellationToken ct, Func<Driver, Booking, bool> action)
    {
        var booking = await repository.GetByIdAsync(id, ct);
        if (booking is null) return false;
        var driver = await driverRepository.GetByIdAsync(booking.Trip.Driver.Id, ct);
        return driver is not null && action(driver, booking) && await repository.UpdateAsync(booking, ct);
    }
}

public class ReviewApplicationService(IReviewRepository repository, IDriverRepository driverRepository, IPassengerRepository passengerRepository, ITripRepository tripRepository)
{
    public async Task<IEnumerable<ReviewModel>> GetAllAsync(CancellationToken ct) => (await repository.GetAllAsync(ct, true)).Select(ModelMapper.Map);
    public async Task<ReviewModel?> GetAsync(Guid id, CancellationToken ct) => (await repository.GetByIdAsync(id, ct)) is { } x ? ModelMapper.Map(x) : null;
    public async Task<ReviewModel?> CreateAsync(CreateReviewModel model, CancellationToken ct)
    {
        var trip = await tripRepository.GetByIdAsync(model.TripId, ct);
        if (trip is null) return null;
        Review? review;
        if (model.AuthorIsDriver)
        {
            var author = await driverRepository.GetByIdAsync(model.AuthorId, ct);
            var target = await passengerRepository.GetByIdAsync(model.TargetId, ct);
            if (author is null || target is null) return null;
            review = author.LeaveReview(target, trip, model.Rating, new ReviewText(model.Text));
        }
        else
        {
            var author = await passengerRepository.GetByIdAsync(model.AuthorId, ct);
            var target = await driverRepository.GetByIdAsync(model.TargetId, ct);
            if (author is null || target is null) return null;
            review = author.LeaveReview(target, trip, model.Rating, new ReviewText(model.Text));
        }
        return (await repository.AddAsync(review, ct)) is { } created ? ModelMapper.Map(created) : null;
    }
    public async Task<bool> UpdateAsync(Guid id, UpdateReviewModel model, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        var changed = entity.ChangeRating(model.Rating) | entity.ChangeText(new ReviewText(model.Text));
        return changed && await repository.UpdateAsync(entity, ct);
    }
    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => repository.DeleteAsync(id, ct);
}
