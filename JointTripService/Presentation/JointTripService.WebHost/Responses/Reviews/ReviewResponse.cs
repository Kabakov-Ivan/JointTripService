namespace JointTripService.WebHost.Responses.Reviews;

public record ReviewResponse(
    Guid Id,
    Guid AuthorId,
    Guid TargetId,
    bool AuthorIsDriver,
    bool TargetIsDriver,
    Guid TripId,
    int Rating,
    string Text,
    DateTime CreationData,
    DateTime? ModificationData);
