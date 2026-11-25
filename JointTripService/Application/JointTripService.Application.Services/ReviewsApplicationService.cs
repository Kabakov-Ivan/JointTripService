using JointTripService.Application.Models;
using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Interfaces;
using JointTripService.ValueObjects;

namespace JointTripService.Application.Services;

public class ReviewApplicationService(IReviewRepository repository, IDriverRepository driverRepository, IPassengerRepository passengerRepository, ITripRepository tripRepository)
{
    public async Task<IEnumerable<ReviewModel>> GetAllAsync(CancellationToken ct) => (await repository.GetAllAsync(ct, true)).Select(ModelMapper.Map);

    public async Task<ReviewModel?> GetAsync(Guid id, CancellationToken ct)
        => (await repository.GetByIdAsync(id, ct)) is { } x ? ModelMapper.Map(x) : null;

    public async Task<ReviewModel?> CreateAsync(CreateReviewModel model, CancellationToken ct)
    {
        var trip = await tripRepository.GetByIdAsync(model.TripId, ct);
        if (trip is null)
            return null;

        Review review;
        if (model.AuthorIsDriver)
        {
            var author = await driverRepository.GetByIdAsync(model.AuthorId, ct);
            var target = await passengerRepository.GetByIdAsync(model.TargetId, ct);
            if (author is null || target is null)
                return null;

            review = author.LeaveReview(target, trip, model.Rating, new ReviewText(model.Text));
        }
        else
        {
            var author = await passengerRepository.GetByIdAsync(model.AuthorId, ct);
            var target = await driverRepository.GetByIdAsync(model.TargetId, ct);
            if (author is null || target is null)
                return null;

            review = author.LeaveReview(target, trip, model.Rating, new ReviewText(model.Text));
        }

        return (await repository.AddAsync(review, ct)) is { } created ? ModelMapper.Map(created) : null;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateReviewModel model, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null)
            return false;

        var changed = entity.ChangeRating(model.Rating) | entity.ChangeText(new ReviewText(model.Text));
        return changed && await repository.UpdateAsync(entity, ct);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => repository.DeleteAsync(id, ct);
}
