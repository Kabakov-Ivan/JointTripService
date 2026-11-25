using JointTripService.Domain.Entities;
using JointTripService.Domain.Enums;
using JointTripService.Domain.Repositories.Abstractions.Interfaces;
using JointTripService.Infrastructure.EntityFramework;
using JointTripService.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace JointTripService.Infrastructure.EntityFramework.RepositoriesEF;

public class DriverRepository(ApplicationDbContext context) : EfRepository<Driver, Guid>(context), IDriverRepository
{
    public Task<Driver?> GetDriverByEmailAsync(string email, CancellationToken cancellationToken)
        => context.Drivers.FirstOrDefaultAsync(x => x.Email == new Email(email), cancellationToken);
}

public class PassengerRepository(ApplicationDbContext context) : EfRepository<Passenger, Guid>(context), IPassengerRepository
{
    public Task<Passenger?> GetPassengerByEmailAsync(string email, CancellationToken cancellationToken)
        => context.Passengers.FirstOrDefaultAsync(x => x.Email == new Email(email), cancellationToken);
}

public class TripRepository(ApplicationDbContext context) : EfRepository<Trip, Guid>(context), ITripRepository
{
    public override async Task<IEnumerable<Trip>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false)
    {
        IQueryable<Trip> query = context.Trips.Include(x => x.Driver);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync(cancellationToken);
    }

    public override Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Trips
            .Include(x => x.Driver)
            .Include("_bookings.Passenger")
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task<IEnumerable<Trip>> GetTripsByRouteAsync(City origin, City destination, CancellationToken cancellationToken, bool asNoTracking = false)
    {
        IQueryable<Trip> query = context.Trips.Where(x => x.Origin == origin && x.Destination == destination).Include(x => x.Driver);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<Trip>> GetPublishedTripsAsync(CancellationToken cancellationToken, bool asNoTracking = false)
    {
        IQueryable<Trip> query = context.Trips.Where(x => x.Status == TripStatus.Published).Include(x => x.Driver);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync(cancellationToken);
    }
}

public class BookingRepository(ApplicationDbContext context) : EfRepository<Booking, Guid>(context), IBookingRepository
{
    public override async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false)
    {
        IQueryable<Booking> query = context.Bookings.Include(x => x.Passenger).Include(x => x.Trip).ThenInclude(x => x.Driver);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync(cancellationToken);
    }

    public override Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Bookings.Include(x => x.Passenger).Include(x => x.Trip).ThenInclude(x => x.Driver).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task<IEnumerable<Booking>> GetBookingsByTripIdAsync(Guid tripId, CancellationToken cancellationToken, bool asNoTracking = false)
    {
        IQueryable<Booking> query = context.Bookings.Where(x => x.Trip.Id == tripId).Include(x => x.Passenger).Include(x => x.Trip);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<Booking>> GetBookingsByPassengerIdAsync(Guid passengerId, CancellationToken cancellationToken, bool asNoTracking = false)
    {
        IQueryable<Booking> query = context.Bookings.Where(x => x.Passenger.Id == passengerId).Include(x => x.Trip);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync(cancellationToken);
    }
}

public class ReviewRepository(ApplicationDbContext context) : EfRepository<Review, Guid>(context), IReviewRepository
{
    public override async Task<IEnumerable<Review>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false)
    {
        IQueryable<Review> query = context.Reviews.Include(x => x.Trip);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync(cancellationToken);
    }

    public override Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Reviews.Include(x => x.Trip).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task<IEnumerable<Review>> GetReviewsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken, bool asNoTracking = false)
    {
        IQueryable<Review> query = context.Reviews.Where(x => x.AuthorId == authorId).Include(x => x.Trip);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<Review>> GetReviewsByTripIdAsync(Guid tripId, CancellationToken cancellationToken, bool asNoTracking = false)
    {
        IQueryable<Review> query = context.Reviews.Where(x => x.Trip.Id == tripId).Include(x => x.Trip);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync(cancellationToken);
    }
}
