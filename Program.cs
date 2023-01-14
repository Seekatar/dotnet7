using FeatureFlags;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.FeatureManagement;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// .NET 7 added for problem details
builder.Services.AddProblemDetails();

// added for feature flags
//builder.Configuration.AddFeatureToggle(); // add my IConfiguration provider

builder.Services.Configure<FeatureManagementOptions>(options =>
{
    options.IgnoreMissingFeatures = false;
    options.IgnoreMissingFeatureFilters = false;
});
builder.Services.AddFeatureManagement().AddFeatureFilter<FeatureFilter>(); // add my feature filter


// .NET 7 added for AUTHN/Z
const string AdminPolicyName = "adminPolicy";
const string AdminRoleName = "admin";

builder.Services.AddAuthentication().AddJwtBearer(); // since only Auth mechanism, don't need to set it as the default
builder.Services.AddAuthorizationBuilder().AddPolicy(AdminPolicyName, policy => policy.RequireRole(AdminRoleName)); // RequireClaim

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
builder.Services.AddSingleton<IService, TestService>();
builder.Services.Configure<SwaggerGeneratorOptions>(opts => opts.InferSecuritySchemes = true);
// end .NET 7 added for authN/Z


builder.Services.AddSingletonFeature<IToggledFeature, ToggledFeatureA, ToggledFeatureB>("NewFeature");
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

// .NET 7 added
// .NET 7 Groups & simplified AUTHN/Z
var client = app.MapGroup("/net7")
    .RequireAuthorization(AdminPolicyName)
    .EnableOpenApiWithAuthentication()
    .WithOpenApi();

client.MapGet("/step/{id:int}", Results<Ok<Step>, NotFound> (int id) =>
{
    if (id < 100) throw new ArgumentException(nameof(id));

    var step = new Step();
    step.Init();
    return TypedResults.Ok(step); // TypedResult new in .NET 7
})
.WithName("GetStep");

client.MapGet("/derived/{id}", Results<Ok<VariableBase>, NotFound> (int id, IService service) =>
{
    var variable = service.GetVariable(id);
    if (variable is null) return TypedResults.NotFound();
    return TypedResults.Ok(variable);
})
.WithName("GetDerived");

client.MapPost("/derived", Results<Created<VariableBase>, BadRequest> (VariableBase variable, IService service, HttpRequest request) =>
{

    int id = service.Add(variable);

    return TypedResults.Created($"{request.Path}/{id}", variable);
})
.WithName("AddDerived");

client.MapGet("/string/{id:int}", Results<Ok<string>, NotFound> (int id) =>
{
    return id switch
    {
        1 => TypedResults.Ok("""This string has "quotes" in it"""),
        2 => TypedResults.Ok("""
        This
        string
        has
        "quotes"
        in
        it
        each
        word
        separate
        indented
"""),
        3 => TypedResults.Ok("""
                This
                string
                has
                "quotes"
                in
                it
                each
                word
                separate
                """),
        4 => TypedResults.Ok(""""This string has """triple quotes""" in it""""),
        5 => TypedResults.Ok($"""
        This string has {id switch
        {
            1 => "1",
            _ => "Not 1"
        }} in it with expression on multiple lines
        """),
        6 => TypedResults.Ok($$"""
        {
            "id": {{id}},
            "name": "Test"
        }
        """),
        _ => TypedResults.NotFound()
    };
})
.WithName("ListDerived");
// end .NET 7 added

app.MapGet("/flags", async (IFeature<IToggledFeature> feature) =>
{
    return (await feature.GetFeature()).GetResult();
});

app.MapGet("/fm", async (IFeatureManager fm, IConfiguration config) =>
{
    var result = new Dictionary<string, string>();
    result.Add("TEST.KEYC", (await fm.IsEnabledAsync("TEST.KEYC")).ToString());
    try
    {
        result.Add("TEST.KEYQ", (await fm.IsEnabledAsync("TEST.KEYQ")).ToString());
    }
    catch
    {
        result.Add("TEST.KEYQ", "Exception!");
    }

    result.Add("WhatTimeIsIt", config["WhatTimeIsIt"] ?? "Not found in IConfiguration");
    result.Add("TEST.KEYC context", (await fm.IsEnabledAsync("TEST.KEYC", new MyFeatureContext())).ToString());
    try
    {
        result.Add("WhatTimeIsItNot", config["WhatTimeIsItNot"] ?? "Not found in IConfiguration");
    }
    catch
    {
        result.Add("WhatTimeIsItNot", "Exception!");
    }
    return result;
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
