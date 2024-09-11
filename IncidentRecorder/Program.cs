using IncidentRecorder;
using IncidentRecorder.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureMiddleware(app);

app.Run();


void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
{
    services.AddControllers();

    // Add DbContext with SQLite
    services.AddDbContext<IncidentContext>(options =>
        options.UseSqlite(configuration.GetConnectionString("IncidentDatabase")));

    // Configure custom validation error handling
    services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = CustomValidationProblemDetailsFactory.CreateProblemDetails(context);
            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });

    // Add API documentation (Swagger)
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}

void ConfigureMiddleware(WebApplication app)
{
    // Configure Swagger for API documentation
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Global exception handling middleware
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
}
