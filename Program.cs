using dotnet7;
using dotnet7.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;
using System.Text.Json.Serialization;

const string AdminPolicyName = "adminPolicy";
const string AdminRoleName = "admin";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// .NET7 added for authN/Z
builder.Services.AddAuthentication().AddJwtBearer(); // since only Auth mechanism, don't need to set it as the default
builder.Services.AddAuthorizationBuilder().AddPolicy(AdminPolicyName, policy => policy.RequireRole(AdminRoleName)); // RequireClaim

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
builder.Services.Configure<SwaggerGeneratorOptions>(opts => opts.InferSecuritySchemes = true);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

var client = app.MapGroup("/derived")
    .RequireAuthorization(AdminPolicyName)
    .EnableOpenApiWithAuthentication()
    .WithOpenApi();

client.MapGet("/{id:int}", Results<Ok<Step>, NotFound> (int id) =>
{
    if (id < 100) return TypedResults.NotFound();

    var step = new Step();
    step.Init();
    var s = JsonSerializer.Serialize(step);
    var step2 = JsonSerializer.Deserialize<Step>(s);
    foreach (var item in step2.Variables)
    {
        app.Logger.LogInformation($"Type is {item.Type switch
        {
            _ => item.GetType().Name
        }}");
    }
    return TypedResults.Ok(step); // TypedResult new in .NET 7
})
.WithName("GetDerived");

client.MapGet("/", Results<Ok<Step>,NotFound> () =>
{
    var step = new Step();
    step.Init();
    var s = JsonSerializer.Serialize(step);
    var step2 = JsonSerializer.Deserialize<Step>(s);
    foreach (var item in step2.Variables)
    {
        app.Logger.LogInformation($"Type is {item.Type switch
        {
            _ => item.GetType().Name
        }}");
    }
    return TypedResults.Ok(step); // TypedResult new in .NET 7
})
.WithName("ListDerived");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
