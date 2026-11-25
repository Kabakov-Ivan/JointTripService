using JointTripService.Application.Models;
using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Interfaces;
using JointTripService.ValueObjects;

namespace JointTripService.Application.Services;

public class PassengerApplicationService(IPassengerRepository repository)
{
    public async Task<IEnumerable<PassengerModel>> GetAllAsync(CancellationToken ct) => (await repository.GetAllAsync(ct, true)).Select(ModelMapper.Map);

    public async Task<PassengerModel?> GetAsync(Guid id, CancellationToken ct)
        => (await repository.GetByIdAsync(id, ct)) is { } x ? ModelMapper.Map(x) : null;

    public async Task<PassengerModel?> CreateAsync(CreatePassengerModel model, CancellationToken ct)
    {
        if (await repository.GetPassengerByEmailAsync(model.Email, ct) is not null)
            return null;

        var entity = new Passenger(Guid.NewGuid(), new FullName(model.FullName), new Email(model.Email));
        return (await repository.AddAsync(entity, ct)) is { } created ? ModelMapper.Map(created) : null;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdatePassengerModel model, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null)
            return false;

        var changed = entity.ChangeFullName(new FullName(model.FullName)) | entity.ChangeEmail(new Email(model.Email));
        return changed && await repository.UpdateAsync(entity, ct);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => repository.DeleteAsync(id, ct);
}
