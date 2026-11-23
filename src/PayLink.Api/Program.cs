using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Phase 1 keeps the API deliberately thin: it boots, it is observable, and CI has
// something real to build and test. Payment endpoints arrive in Phase 2.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PayLink API",
        Version = "v1",
        Description = "Bill-payment and wallet backend. Phase 1 foundation.",
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

// Exposed so integration tests can host the API with WebApplicationFactory.
public partial class Program
{
}
