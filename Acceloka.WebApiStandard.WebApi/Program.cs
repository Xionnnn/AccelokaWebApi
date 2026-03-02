using Acceloka.WebApiStandard.Commons.Behaviors;
using Acceloka.WebApiStandard.Entities;
using Acceloka.WebApiStandard.RequestHandlers.ManageTickets;
using Acceloka.WebApiStandard.Validators.ManageTickets;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.MinimumLevel.Information()
      .WriteTo.File(
          path: "logs/Log-.txt",
          rollingInterval: RollingInterval.Day,
          shared: true);
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// MediatR (scan assembly Application)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetAvailableTicketHandler>());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<BookTicketHandler>());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ViewTicketDetailHandler>());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RevokeBookedTicketHandler>());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetBookingHandler>());

// FluentValidation (scan assembly Application)
builder.Services.AddValidatorsFromAssemblyContaining<GetAvailableTicketValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BookTicketValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ViewTicketDetailValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RevokeBookedTicketValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetBookingValidator>();

// FluentValidation via MediatR pipeline behavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    // Atau pake policy spesifik untuk production:
    // options.AddPolicy("AllowSpecificOrigins", policy =>
    // {
    //     policy.WithOrigins("https://yourdomain.com", "http://localhost:3000")
    //           .AllowAnyMethod()
    //           .AllowAnyHeader()
    //           .AllowCredentials();
    // });
});

//Db Connection
var connectionString = builder.Configuration.GetConnectionString("PgSQLDB");

builder.Services.AddDbContext<AccelokaDbContext>(opt => opt.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Error Compliant
app.UseExceptionHandler("/error");

// Enable CORS - harus sebelum UseAuthorization
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
