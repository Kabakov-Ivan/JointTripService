using FluentValidation;
using JointTripService.WebHost.Requests.Bookings;

namespace JointTripService.WebHost.Validators.Bookings;

public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(request => request.PassengerId).NotEmpty();
        RuleFor(request => request.TripId).NotEmpty();
        RuleFor(request => request.SeatsCount).GreaterThan(0);
    }
}
