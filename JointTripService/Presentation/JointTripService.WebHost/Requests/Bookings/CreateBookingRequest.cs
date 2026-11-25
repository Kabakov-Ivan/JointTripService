namespace JointTripService.WebHost.Requests.Bookings;

public record CreateBookingRequest(Guid PassengerId, Guid TripId, int SeatsCount);
