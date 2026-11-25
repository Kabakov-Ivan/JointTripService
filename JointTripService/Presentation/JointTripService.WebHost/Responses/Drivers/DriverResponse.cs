namespace JointTripService.WebHost.Responses.Drivers;

public record DriverResponse(Guid Id, string FullName, string Email, DateTime CreationData, DateTime? ModificationData);
