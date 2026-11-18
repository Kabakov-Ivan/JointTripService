namespace JointTripService.Application.Models;

public record DriverModel(Guid Id, string FullName, string Email, DateTime CreationData, DateTime? ModificationData);
public record PassengerModel(Guid Id, string FullName, string Email, DateTime CreationData, DateTime? ModificationData);
public record TripModel(Guid Id, Guid DriverId, string Origin, string Destination, DateTime DepartureAt, int SeatsCount, int AvailableSeats, string? Description, string Status, DateTime CreationData, DateTime? ModificationData);
public record BookingModel(Guid Id, Guid PassengerId, Guid TripId, int SeatsCount, string Status, DateTime CreationData, DateTime? ModificationData);
public record ReviewModel(Guid Id, Guid AuthorId, Guid TargetId, bool AuthorIsDriver, bool TargetIsDriver, Guid TripId, int Rating, string Text, DateTime CreationData, DateTime? ModificationData);

public record CreateDriverModel(string FullName, string Email);
public record CreatePassengerModel(string FullName, string Email);
public record CreateTripModel(Guid DriverId, string Origin, string Destination, DateTime DepartureAt, int SeatsCount, string? Description);
public record UpdateTripModel(string Origin, string Destination, DateTime DepartureAt, int SeatsCount, string? Description);
public record CreateBookingModel(Guid PassengerId, Guid TripId, int SeatsCount);
public record CreateReviewModel(Guid AuthorId, Guid TargetId, Guid TripId, bool AuthorIsDriver, int Rating, string Text);
public record UpdateDriverModel(string FullName, string Email);
public record UpdatePassengerModel(string FullName, string Email);
public record UpdateReviewModel(int Rating, string Text);
