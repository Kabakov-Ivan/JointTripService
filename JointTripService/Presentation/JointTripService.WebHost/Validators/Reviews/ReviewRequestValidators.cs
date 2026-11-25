using FluentValidation;
using JointTripService.ValueObjects.Validators;
using JointTripService.WebHost.Requests.Reviews;

namespace JointTripService.WebHost.Validators.Reviews;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(request => request.AuthorId).NotEmpty();
        RuleFor(request => request.TargetId).NotEmpty();
        RuleFor(request => request.TripId).NotEmpty();
        RuleFor(request => request).Must(request => request.AuthorId != request.TargetId)
            .WithMessage("Автор и адресат отзыва должны быть разными пользователями.");
        RuleFor(request => request.Rating).InclusiveBetween(1, 5);
        RuleFor(request => request.Text).Must(BeValidText)
            .WithMessage($"Текст отзыва обязателен и должен содержать не более {ReviewTextValidator.MAX_LENGTH} символов.");
    }

    internal static bool BeValidText(string? value)
        => !string.IsNullOrWhiteSpace(value)
           && value.Trim().Length <= ReviewTextValidator.MAX_LENGTH;
}

public class UpdateReviewRequestValidator : AbstractValidator<UpdateReviewRequest>
{
    public UpdateReviewRequestValidator()
    {
        RuleFor(request => request.Rating).InclusiveBetween(1, 5);
        RuleFor(request => request.Text).Must(CreateReviewRequestValidator.BeValidText)
            .WithMessage($"Текст отзыва обязателен и должен содержать не более {ReviewTextValidator.MAX_LENGTH} символов.");
    }
}
