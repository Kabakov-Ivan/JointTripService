namespace JointTripService.WebHost.Responses.Trips;

public record TripResponse(
    Guid Id,
    Guid DriverId,
    string Origin,
    string Destination,
    DateTime DepartureAt,
    int SeatsCount,
    int AvailableSeats,
    string? Description,
    string Status,
    DateTime CreationData,
    DateTime? ModificationData);
