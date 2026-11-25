using FluentValidation;
using JointTripService.ValueObjects.Validators;
using JointTripService.WebHost.Requests.Passengers;
using JointTripService.WebHost.Validators.Drivers;

namespace JointTripService.WebHost.Validators.Passengers;

public class CreatePassengerRequestValidator : AbstractValidator<CreatePassengerRequest>
{
    public CreatePassengerRequestValidator()
    {
        RuleFor(request => request.FullName).Must(CreateDriverRequestValidator.BeValidFullName)
            .WithMessage($"Имя должно содержать от {FullNameValidator.MIN_LENGTH} до {FullNameValidator.MAX_LENGTH} символов.");
        RuleFor(request => request.Email).Must(CreateDriverRequestValidator.BeValidEmail)
            .WithMessage($"Укажите корректный адрес электронной почты длиной не более {EmailValidator.MAX_LENGTH} символов.");
    }
}

public class UpdatePassengerRequestValidator : AbstractValidator<UpdatePassengerRequest>
{
    public UpdatePassengerRequestValidator()
    {
        RuleFor(request => request.FullName).Must(CreateDriverRequestValidator.BeValidFullName)
            .WithMessage($"Имя должно содержать от {FullNameValidator.MIN_LENGTH} до {FullNameValidator.MAX_LENGTH} символов.");
        RuleFor(request => request.Email).Must(CreateDriverRequestValidator.BeValidEmail)
            .WithMessage($"Укажите корректный адрес электронной почты длиной не более {EmailValidator.MAX_LENGTH} символов.");
    }
}
