namespace JointTripService.WebHost.Responses.Bookings;

public record BookingResponse(
    Guid Id,
    Guid PassengerId,
    Guid TripId,
    int SeatsCount,
    string Status,
    DateTime CreationData,
    DateTime? ModificationData);
