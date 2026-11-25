using JointTripService.Application.Models;
using JointTripService.Application.Services;
using JointTripService.WebHost.Requests.Bookings;
using JointTripService.WebHost.Requests.Drivers;
using JointTripService.WebHost.Requests.Passengers;
using JointTripService.WebHost.Requests.Reviews;
using JointTripService.WebHost.Requests.Trips;
using JointTripService.WebHost.Responses;
using JointTripService.WebHost.Responses.Bookings;
using JointTripService.WebHost.Responses.Drivers;
using JointTripService.WebHost.Responses.Passengers;
using JointTripService.WebHost.Responses.Reviews;
using JointTripService.WebHost.Responses.Trips;
using Microsoft.AspNetCore.Mvc;

namespace JointTripService.WebHost.Controllers;

[ApiController]
[Route("api/drivers")]
public class DriversController(DriverApplicationService service) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DriverResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok((await service.GetAllAsync(ct)).Select(Map));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DriverResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await service.GetAsync(id, ct)) is { } driver ? Ok(Map(driver)) : NotFound(NotFoundError("водитель", id));

    [HttpPost]
    [ProducesResponseType(typeof(DriverResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(CreateDriverRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(new CreateDriverModel(request.FullName, request.Email), ct);
        return created is null
            ? Conflict(new ApiErrorResponse("driver_conflict", "Водитель с таким адресом электронной почты уже существует."))
            : CreatedAtAction(nameof(Get), new { id = created.Id }, Map(created));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, UpdateDriverRequest request, CancellationToken ct)
        => await service.UpdateAsync(id, new UpdateDriverModel(request.FullName, request.Email), ct)
            ? NoContent()
            : NotFound(NotFoundError("водитель", id));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await service.DeleteAsync(id, ct) ? NoContent() : NotFound(NotFoundError("водитель", id));

    private static DriverResponse Map(DriverModel model)
        => new(model.Id, model.FullName, model.Email, model.CreationData, model.ModificationData);
}

[ApiController]
[Route("api/passengers")]
public class PassengersController(PassengerApplicationService service) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PassengerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok((await service.GetAllAsync(ct)).Select(Map));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PassengerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await service.GetAsync(id, ct)) is { } passenger ? Ok(Map(passenger)) : NotFound(NotFoundError("пассажир", id));

    [HttpPost]
    [ProducesResponseType(typeof(PassengerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(CreatePassengerRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(new CreatePassengerModel(request.FullName, request.Email), ct);
        return created is null
            ? Conflict(new ApiErrorResponse("passenger_conflict", "Пассажир с таким адресом электронной почты уже существует."))
            : CreatedAtAction(nameof(Get), new { id = created.Id }, Map(created));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, UpdatePassengerRequest request, CancellationToken ct)
        => await service.UpdateAsync(id, new UpdatePassengerModel(request.FullName, request.Email), ct)
            ? NoContent()
            : NotFound(NotFoundError("пассажир", id));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await service.DeleteAsync(id, ct) ? NoContent() : NotFound(NotFoundError("пассажир", id));

    private static PassengerResponse Map(PassengerModel model)
        => new(model.Id, model.FullName, model.Email, model.CreationData, model.ModificationData);
}

[ApiController]
[Route("api/trips")]
public class TripsController(TripApplicationService service) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TripResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok((await service.GetAllAsync(ct)).Select(Map));

    [HttpGet("published")]
    [ProducesResponseType(typeof(IEnumerable<TripResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublished(CancellationToken ct)
        => Ok((await service.GetPublishedAsync(ct)).Select(Map));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TripResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await service.GetAsync(id, ct)) is { } trip ? Ok(Map(trip)) : NotFound(NotFoundError("поездка", id));

    [HttpPost]
    [ProducesResponseType(typeof(TripResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(CreateTripRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(
            new CreateTripModel(request.DriverId, request.Origin, request.Destination, request.DepartureAt, request.SeatsCount, request.Description),
            ct);
        return created is null
            ? NotFound(NotFoundError("водитель", request.DriverId))
            : CreatedAtAction(nameof(Get), new { id = created.Id }, Map(created));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, UpdateTripRequest request, CancellationToken ct)
        => await service.UpdateAsync(id, new UpdateTripModel(request.Origin, request.Destination, request.DepartureAt, request.SeatsCount, request.Description), ct)
            ? NoContent()
            : NotFound(NotFoundError("поездка", id));

    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
        => await service.PublishAsync(id, ct) ? NoContent() : NotFound(NotFoundError("поездка", id));

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        => await service.CancelAsync(id, ct) ? NoContent() : NotFound(NotFoundError("поездка", id));

    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
        => await service.CompleteAsync(id, ct) ? NoContent() : NotFound(NotFoundError("поездка", id));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await service.DeleteAsync(id, ct) ? NoContent() : NotFound(NotFoundError("поездка", id));

    private static TripResponse Map(TripModel model)
        => new(model.Id, model.DriverId, model.Origin, model.Destination, model.DepartureAt, model.SeatsCount, model.AvailableSeats, model.Description, model.Status, model.CreationData, model.ModificationData);
}

[ApiController]
[Route("api/bookings")]
public class BookingsController(BookingApplicationService service) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok((await service.GetAllAsync(ct)).Select(Map));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await service.GetAsync(id, ct)) is { } booking ? Ok(Map(booking)) : NotFound(NotFoundError("бронирование", id));

    [HttpPost]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(CreateBookingRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(new CreateBookingModel(request.PassengerId, request.TripId, request.SeatsCount), ct);
        return created is null
            ? NotFound(new ApiErrorResponse("related_entity_not_found", "Пассажир или поездка не найдены."))
            : CreatedAtAction(nameof(Get), new { id = created.Id }, Map(created));
    }

    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken ct)
        => await service.ApproveAsync(id, ct) ? NoContent() : NotFound(NotFoundError("бронирование", id));

    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Reject(Guid id, CancellationToken ct)
        => await service.RejectAsync(id, ct) ? NoContent() : NotFound(NotFoundError("бронирование", id));

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        => await service.CancelAsync(id, ct) ? NoContent() : NotFound(NotFoundError("бронирование", id));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await service.DeleteAsync(id, ct) ? NoContent() : NotFound(NotFoundError("бронирование", id));

    private static BookingResponse Map(BookingModel model)
        => new(model.Id, model.PassengerId, model.TripId, model.SeatsCount, model.Status, model.CreationData, model.ModificationData);
}

[ApiController]
[Route("api/reviews")]
public class ReviewsController(ReviewApplicationService service) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReviewResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok((await service.GetAllAsync(ct)).Select(Map));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await service.GetAsync(id, ct)) is { } review ? Ok(Map(review)) : NotFound(NotFoundError("отзыв", id));

    [HttpPost]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(CreateReviewRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(
            new CreateReviewModel(request.AuthorId, request.TargetId, request.TripId, request.AuthorIsDriver, request.Rating, request.Text),
            ct);
        return created is null
            ? NotFound(new ApiErrorResponse("related_entity_not_found", "Автор, адресат отзыва или поездка не найдены."))
            : CreatedAtAction(nameof(Get), new { id = created.Id }, Map(created));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, UpdateReviewRequest request, CancellationToken ct)
        => await service.UpdateAsync(id, new UpdateReviewModel(request.Rating, request.Text), ct)
            ? NoContent()
            : NotFound(NotFoundError("отзыв", id));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await service.DeleteAsync(id, ct) ? NoContent() : NotFound(NotFoundError("отзыв", id));

    private static ReviewResponse Map(ReviewModel model)
        => new(model.Id, model.AuthorId, model.TargetId, model.AuthorIsDriver, model.TargetIsDriver, model.TripId, model.Rating, model.Text, model.CreationData, model.ModificationData);
}

public abstract class ApiControllerBase : ControllerBase
{
    protected static ApiErrorResponse NotFoundError(string entity, Guid id)
        => new("not_found", $"Объект «{entity}» с идентификатором «{id}» не найден.");
}
