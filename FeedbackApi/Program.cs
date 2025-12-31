using FeedbackApi.Data;
using FeedbackApi.Endpoints;
using FeedbackApi.Services;
using FeedbackApi.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Feedback API",
        Version = "v1",
        Description = "MVP Review/Feedback service for apartment rentals"
    });

    options.AddSecurityDefinition("UserId", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-User-Id",
        Description = "User ID (GUID) for authentication. Use one of the seeded users:\n" +
                      "• John Doe: 11111111-1111-1111-1111-111111111111\n" +
                      "• Jane Smith: 22222222-2222-2222-2222-222222222222\n" +
                      "• Bob Wilson: 33333333-3333-3333-3333-333333333333"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "UserId"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<FeedbackDbContext>(options =>
    options.UseInMemoryDatabase("FeedbackDb"));

builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddSingleton<ReviewValidator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FeedbackDbContext>();
    DataSeeder.Seed(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Feedback API v1");
    });
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.MapReviewEndpoints();
app.MapApartmentEndpoints();
app.MapUserEndpoints();

app.Run();
