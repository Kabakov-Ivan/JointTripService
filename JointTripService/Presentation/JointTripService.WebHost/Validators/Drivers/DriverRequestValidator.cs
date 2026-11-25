using FluentValidation;
using JointTripService.ValueObjects.Validators;
using JointTripService.WebHost.Requests.Drivers;

namespace JointTripService.WebHost.Validators.Drivers;

public class CreateDriverRequestValidator : AbstractValidator<CreateDriverRequest>
{
    public CreateDriverRequestValidator()
    {
        RuleFor(request => request.FullName).Must(BeValidFullName)
            .WithMessage($"Имя должно содержать от {FullNameValidator.MIN_LENGTH} до {FullNameValidator.MAX_LENGTH} символов.");
        RuleFor(request => request.Email).Must(BeValidEmail)
            .WithMessage($"Укажите корректный адрес электронной почты длиной не более {EmailValidator.MAX_LENGTH} символов.");
    }

    internal static bool BeValidFullName(string? value)
        => !string.IsNullOrWhiteSpace(value)
           && value.Trim().Length >= FullNameValidator.MIN_LENGTH
           && value.Trim().Length <= FullNameValidator.MAX_LENGTH;

    internal static bool BeValidEmail(string? value)
        => !string.IsNullOrWhiteSpace(value)
           && value.Trim().Length <= EmailValidator.MAX_LENGTH
           && value.Trim().Contains('@')
           && !value.Trim().StartsWith('@')
           && !value.Trim().EndsWith('@');
}

public class UpdateDriverRequestValidator : AbstractValidator<UpdateDriverRequest>
{
    public UpdateDriverRequestValidator()
    {
        RuleFor(request => request.FullName).Must(CreateDriverRequestValidator.BeValidFullName)
            .WithMessage($"Имя должно содержать от {FullNameValidator.MIN_LENGTH} до {FullNameValidator.MAX_LENGTH} символов.");
        RuleFor(request => request.Email).Must(CreateDriverRequestValidator.BeValidEmail)
            .WithMessage($"Укажите корректный адрес электронной почты длиной не более {EmailValidator.MAX_LENGTH} символов.");
    }
}
