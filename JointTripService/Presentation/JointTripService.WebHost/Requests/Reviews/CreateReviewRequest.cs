namespace JointTripService.WebHost.Requests.Reviews;

public record CreateReviewRequest(
    Guid AuthorId,
    Guid TargetId,
    Guid TripId,
    bool AuthorIsDriver,
    int Rating,
    string Text);
