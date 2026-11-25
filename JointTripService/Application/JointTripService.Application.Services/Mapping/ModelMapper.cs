using JointTripService.Application.Models;
using JointTripService.Domain.Entities;

namespace JointTripService.Application.Services;

public static class ModelMapper
{
    public static DriverModel Map(Driver x) => new(x.Id, x.FullName.Value, x.Email.Value, x.CreationData, x.ModificationData);
    public static PassengerModel Map(Passenger x) => new(x.Id, x.FullName.Value, x.Email.Value, x.CreationData, x.ModificationData);
    public static TripModel Map(Trip x) => new(x.Id, x.Driver.Id, x.Origin.Value, x.Destination.Value, x.DepartureAt, x.SeatsCount.Value, x.AvailableSeats, x.Description, x.Status.ToString(), x.CreationData, x.ModificationData);
    public static BookingModel Map(Booking x) => new(x.Id, x.Passenger.Id, x.Trip.Id, x.SeatsCount.Value, x.Status.ToString(), x.CreationData, x.ModificationData);
    public static ReviewModel Map(Review x) => new(x.Id, x.AuthorId, x.TargetId, x.AuthorIsDriver, x.TargetIsDriver, x.Trip.Id, x.Rating, x.Text.Value, x.CreationData, x.ModificationData);
}
