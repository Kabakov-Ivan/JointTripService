namespace JointTripService.WebHost.Responses.Passengers;

public record PassengerResponse(Guid Id, string FullName, string Email, DateTime CreationData, DateTime? ModificationData);
