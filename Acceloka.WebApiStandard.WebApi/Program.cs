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
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<DeleteBookedTicketHandler>());

// FluentValidation (scan assembly Application)
builder.Services.AddValidatorsFromAssemblyContaining<GetAvailableTicketValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BookTicketValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ViewTicketDetailValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteBookedTicketValidator>();

// FluentValidation via MediatR pipeline behavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
