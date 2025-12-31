using FeedbackApi.Data;
using FeedbackApi.Endpoints;
using FeedbackApi.Services;
using FeedbackApi.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Feedback API",
        Version = "v1",
        Description = "MVP Review/Feedback service for apartment rentals"
    });

    // Add X-User-Id header authentication
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

// Configure EF Core In-Memory Database
builder.Services.AddDbContext<FeedbackDbContext>(options =>
    options.UseInMemoryDatabase("FeedbackDb"));

// Register services
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddSingleton<ReviewValidator>();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FeedbackDbContext>();
    DataSeeder.Seed(context);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Feedback API v1");
    });
}

app.UseHttpsRedirection();

// Map endpoints
app.MapReviewEndpoints();
app.MapApartmentEndpoints();
app.MapUserEndpoints();

app.Run();
