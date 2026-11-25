using Microsoft.EntityFrameworkCore;
using JointTripService.Infrastructure.EntityFramework;
using JointTripService.WebHost.Helpers;
using Microsoft.OpenApi;
using JointTripService.Application.Services;
using JointTripService.Domain.Entities;
using JointTripService.Domain.Repositories.Abstractions.Interfaces;
using JointTripService.Infrastructure.EntityFramework.RepositoriesEF;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using JointTripService.WebHost.Responses;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString(nameof(ApplicationDbContext));

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Не настроена строка подключения для ApplicationDbContext.");
}

builder.Services.AddNpgsql<ApplicationDbContext>(connectionString, options =>
{
    options.MigrationsAssembly("JointTripService.Infrastructure.EntityFramework");

});

builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Сервис совместных поездок",
            Description = "API для создания и управления поездками, бронированиями и отзывами."
        });
    });

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseNpgsql(connectionString);
    });

builder.Services.AddControllers();
ValidatorOptions.Global.LanguageManager.Culture = CultureInfo.GetCultureInfo("ru");
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors
                    .Select(error => error.Exception is null ? error.ErrorMessage : "Значение имеет неверный формат.")
                    .ToArray());

        return new BadRequestObjectResult(new ApiErrorResponse(
            "validation_error",
            "Запрос содержит ошибки проверки.",
            errors));
    };
});
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IPassengerRepository, PassengerRepository>();
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<DriverApplicationService>();
builder.Services.AddScoped<PassengerApplicationService>();
builder.Services.AddScoped<TripApplicationService>();
builder.Services.AddScoped<BookingApplicationService>();
builder.Services.AddScoped<ReviewApplicationService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
app.UseMiddleware<ApiExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.MigrateDatabase<ApplicationDbContext>();

app.Run();