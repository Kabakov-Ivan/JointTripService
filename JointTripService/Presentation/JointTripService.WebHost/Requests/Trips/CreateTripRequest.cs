namespace JointTripService.WebHost.Requests.Trips;

public record CreateTripRequest(
    Guid DriverId,
    string Origin,
    string Destination,
    DateTime DepartureAt,
    int SeatsCount,
    string? Description);
