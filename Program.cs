using FeatureFlags;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.FeatureManagement;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;

Environment.SetEnvironmentVariable("FeatureManagement__PLAIN.KEYC", "true");
Environment.SetEnvironmentVariable("FeatureManagement__CNTXT.KEYC__EnabledFor__0__Name", "FilterMe");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// .NET 7 added for problem details
builder.Services.AddProblemDetails();

// added for feature flags
builder.Configuration.AddFeatureToggle(); // add my IConfiguration provider

builder.Services.Configure<FeatureManagementOptions>(options =>
{
    options.IgnoreMissingFeatures = false;
    options.IgnoreMissingFeatureFilters = false;
});
builder.Services.AddSingleton<IFeatureFlags,FeatureFlagService>();
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

var set = true;
app.MapGet("/fm", async (IFeatureManager fm, IConfiguration config) =>
{
    var result = new Dictionary<string, string>();
    for (char x = 'A'; x < 'E'; x++)
    {
        try
        {
            result.Add($"PLAIN.KEY{x}", (await fm.IsEnabledAsync($"PLAIN.KEY{x}")).ToString());
        }
        catch (Exception ex)
        {
            result.Add($"PLAIN.KEY{x}", $"Exception! {ex.Message}");
        }
    }
    for (char x = 'A'; x < 'E'; x++)
    {
        try
        {
            result.Add($"CNTXT.KEY{x} Context", (await fm.IsEnabledAsync($"CNTXT.KEY{x}", new MyFeatureContext() { EnableMe = set })).ToString());
        }
        catch (Exception ex)
        {
            result.Add($"CNTXT.KEY{x} Context", $"Exception! {ex.Message}");
        }
    }
    result.Add("Set is", set.ToString());
    set = !set;

    // Test IConfiguration not found
    result.Add("WhatTimeIsIt", config["WhatTimeIsIt"] ?? "Not found in IConfiguration");
    result.Add("WhatTimeIsItNot", config["WhatTimeIsItNot"] ?? "Not found in IConfiguration"); // never throws, just returns null

    return result;
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
