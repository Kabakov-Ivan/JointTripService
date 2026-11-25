using JointTripService.Domain.Exceptions;
using JointTripService.WebHost.Responses;

namespace JointTripService.WebHost.Helpers;

public class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (DomainException exception)
        {
            logger.LogWarning(exception, "Доменное правило отклонило запрос.");
            await WriteErrorAsync(context, StatusCodes.Status409Conflict, "domain_conflict", exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            logger.LogWarning(exception, "Операция недопустима в текущем состоянии объекта.");
            await WriteErrorAsync(context, StatusCodes.Status409Conflict, "operation_conflict", exception.Message);
        }
        catch (ArgumentException exception)
        {
            logger.LogWarning(exception, "В запросе передан некорректный аргумент.");
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "invalid_argument", "В запросе переданы некорректные данные.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string code, string message)
    {
        if (context.Response.HasStarted)
            throw new InvalidOperationException("Ответ уже начал отправляться клиенту.");

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new ApiErrorResponse(code, message));
    }
}
