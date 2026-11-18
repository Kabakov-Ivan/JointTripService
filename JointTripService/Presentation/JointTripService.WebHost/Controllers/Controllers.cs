using JointTripService.Application.Models;
using JointTripService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace JointTripService.WebHost.Controllers;

[ApiController]
[Route("api/drivers")]
public class DriversController(DriverApplicationService service) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await service.GetAsync(id, ct)) is { } x ? Ok(x) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(CreateDriverModel model, CancellationToken ct) => (await service.CreateAsync(model, ct)) is { } x ? CreatedAtAction(nameof(Get), new { x.Id }, x) : BadRequest("Driver cannot be created.");
    [HttpPut("{id:guid}")] public async Task<IActionResult> Update(Guid id, UpdateDriverModel model, CancellationToken ct) => await service.UpdateAsync(id, model, ct) ? NoContent() : NotFound();
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id, CancellationToken ct) => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}

[ApiController]
[Route("api/passengers")]
public class PassengersController(PassengerApplicationService service) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await service.GetAsync(id, ct)) is { } x ? Ok(x) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(CreatePassengerModel model, CancellationToken ct) => (await service.CreateAsync(model, ct)) is { } x ? CreatedAtAction(nameof(Get), new { x.Id }, x) : BadRequest("Passenger cannot be created.");
    [HttpPut("{id:guid}")] public async Task<IActionResult> Update(Guid id, UpdatePassengerModel model, CancellationToken ct) => await service.UpdateAsync(id, model, ct) ? NoContent() : NotFound();
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id, CancellationToken ct) => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}

[ApiController]
[Route("api/trips")]
public class TripsController(TripApplicationService service) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("published")] public async Task<IActionResult> GetPublished(CancellationToken ct) => Ok(await service.GetPublishedAsync(ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await service.GetAsync(id, ct)) is { } x ? Ok(x) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(CreateTripModel model, CancellationToken ct) => (await service.CreateAsync(model, ct)) is { } x ? CreatedAtAction(nameof(Get), new { x.Id }, x) : BadRequest("Trip cannot be created.");
    [HttpPut("{id:guid}")] public async Task<IActionResult> Update(Guid id, UpdateTripModel model, CancellationToken ct) => await service.UpdateAsync(id, model, ct) ? NoContent() : NotFound();
    [HttpPost("{id:guid}/publish")] public async Task<IActionResult> Publish(Guid id, CancellationToken ct) => await service.PublishAsync(id, ct) ? NoContent() : BadRequest();
    [HttpPost("{id:guid}/cancel")] public async Task<IActionResult> Cancel(Guid id, CancellationToken ct) => await service.CancelAsync(id, ct) ? NoContent() : BadRequest();
    [HttpPost("{id:guid}/complete")] public async Task<IActionResult> Complete(Guid id, CancellationToken ct) => await service.CompleteAsync(id, ct) ? NoContent() : BadRequest();
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id, CancellationToken ct) => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}

[ApiController]
[Route("api/bookings")]
public class BookingsController(BookingApplicationService service) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await service.GetAsync(id, ct)) is { } x ? Ok(x) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(CreateBookingModel model, CancellationToken ct) => (await service.CreateAsync(model, ct)) is { } x ? CreatedAtAction(nameof(Get), new { x.Id }, x) : BadRequest("Booking cannot be created.");
    [HttpPost("{id:guid}/approve")] public async Task<IActionResult> Approve(Guid id, CancellationToken ct) => await service.ApproveAsync(id, ct) ? NoContent() : BadRequest();
    [HttpPost("{id:guid}/reject")] public async Task<IActionResult> Reject(Guid id, CancellationToken ct) => await service.RejectAsync(id, ct) ? NoContent() : BadRequest();
    [HttpPost("{id:guid}/cancel")] public async Task<IActionResult> Cancel(Guid id, CancellationToken ct) => await service.CancelAsync(id, ct) ? NoContent() : BadRequest();
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id, CancellationToken ct) => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}

[ApiController]
[Route("api/reviews")]
public class ReviewsController(ReviewApplicationService service) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await service.GetAsync(id, ct)) is { } x ? Ok(x) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(CreateReviewModel model, CancellationToken ct) => (await service.CreateAsync(model, ct)) is { } x ? CreatedAtAction(nameof(Get), new { x.Id }, x) : BadRequest("Review cannot be created.");
    [HttpPut("{id:guid}")] public async Task<IActionResult> Update(Guid id, UpdateReviewModel model, CancellationToken ct) => await service.UpdateAsync(id, model, ct) ? NoContent() : NotFound();
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id, CancellationToken ct) => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
