namespace JointTripService.WebHost.Responses;

public record ApiErrorResponse(string Code, string Message, IDictionary<string, string[]>? Errors = null);
