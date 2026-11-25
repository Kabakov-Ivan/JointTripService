using FluentValidation;
using JointTripService.ValueObjects.Validators;
using JointTripService.WebHost.Requests.Trips;

namespace JointTripService.WebHost.Validators.Trips;

public class CreateTripRequestValidator : AbstractValidator<CreateTripRequest>
{
    public CreateTripRequestValidator()
    {
        RuleFor(request => request.DriverId).NotEmpty();
        RuleFor(request => request.Origin).Must(BeValidCity)
            .WithMessage($"Город отправления должен содержать от {CityValidator.MIN_LENGTH} до {CityValidator.MAX_LENGTH} символов.");
        RuleFor(request => request.Destination).Must(BeValidCity)
            .WithMessage($"Город назначения должен содержать от {CityValidator.MIN_LENGTH} до {CityValidator.MAX_LENGTH} символов.");
        RuleFor(request => request).Must(request => !string.Equals(request.Origin?.Trim(), request.Destination?.Trim(), StringComparison.Ordinal))
            .WithMessage("Город отправления и город назначения должны различаться.");
        RuleFor(request => request.SeatsCount).GreaterThan(0);
    }

    internal static bool BeValidCity(string? value)
        => !string.IsNullOrWhiteSpace(value)
           && value.Trim().Length >= CityValidator.MIN_LENGTH
           && value.Trim().Length <= CityValidator.MAX_LENGTH;
}

public class UpdateTripRequestValidator : AbstractValidator<UpdateTripRequest>
{
    public UpdateTripRequestValidator()
    {
        RuleFor(request => request.Origin).Must(CreateTripRequestValidator.BeValidCity)
            .WithMessage($"Город отправления должен содержать от {CityValidator.MIN_LENGTH} до {CityValidator.MAX_LENGTH} символов.");
        RuleFor(request => request.Destination).Must(CreateTripRequestValidator.BeValidCity)
            .WithMessage($"Город назначения должен содержать от {CityValidator.MIN_LENGTH} до {CityValidator.MAX_LENGTH} символов.");
        RuleFor(request => request).Must(request => !string.Equals(request.Origin?.Trim(), request.Destination?.Trim(), StringComparison.Ordinal))
            .WithMessage("Город отправления и город назначения должны различаться.");
        RuleFor(request => request.SeatsCount).GreaterThan(0);
    }
}
