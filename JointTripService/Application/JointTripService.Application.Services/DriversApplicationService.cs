using JointTripService.Application.Models;
using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Interfaces;
using JointTripService.ValueObjects;

namespace JointTripService.Application.Services;

public class DriverApplicationService(IDriverRepository repository)
{
    public async Task<IEnumerable<DriverModel>> GetAllAsync(CancellationToken ct) => (await repository.GetAllAsync(ct, true)).Select(ModelMapper.Map);

    public async Task<DriverModel?> GetAsync(Guid id, CancellationToken ct)
        => (await repository.GetByIdAsync(id, ct)) is { } x ? ModelMapper.Map(x) : null;

    public async Task<DriverModel?> CreateAsync(CreateDriverModel model, CancellationToken ct)
    {
        if (await repository.GetDriverByEmailAsync(model.Email, ct) is not null)
            return null;

        var entity = new Driver(Guid.NewGuid(), new FullName(model.FullName), new Email(model.Email));
        return (await repository.AddAsync(entity, ct)) is { } created ? ModelMapper.Map(created) : null;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateDriverModel model, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null)
            return false;

        var changed = entity.ChangeFullName(new FullName(model.FullName)) | entity.ChangeEmail(new Email(model.Email));
        return changed && await repository.UpdateAsync(entity, ct);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => repository.DeleteAsync(id, ct);
}
