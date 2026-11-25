namespace JointTripService.WebHost.Requests.Trips;

public record UpdateTripRequest(
    string Origin,
    string Destination,
    DateTime DepartureAt,
    int SeatsCount,
    string? Description);
