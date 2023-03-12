using dotnet7.FeatureFlags;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
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


builder.Services.AddSingleton<IFeatureFlagService,FeatureFlagService>();
builder.Services.AddSingleton<FeatureManager>();
builder.Services.AddSingleton<IFeatureManager>( (svc) => svc.GetRequiredService<FeatureManager>());
builder.Services.AddSingleton<IFeatureFlag>((svc) => svc.GetRequiredService<FeatureManager>());
builder.Services.AddScoped<FeatureManagerSnapshot>();
builder.Services.AddScoped<IFeatureManagerSnapshot>( (svc) => svc.GetRequiredService<FeatureManagerSnapshot>());
builder.Services.AddScoped<IFeatureFlagSnapshot>((svc) => svc.GetRequiredService<FeatureManagerSnapshot>());

builder.Services.AddSingletonFeature<IToggledFeature, ToggledFeatureA, ToggledFeatureB>("PLAIN.KEYC");
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

#region dotnet7_features
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

client.MapGet("/derived/{id}", Results<Ok<VariableBase>, NotFound> (int id, IService service) => {
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
#endregion

#region Features

var featureTags = new List<OpenApiTag> { new OpenApiTag() { Name = "Features" } };

app.MapGet("/fm/injected", async (IFeature<IToggledFeature> feature) =>
{
    return (await feature.GetFeature()).GetResult();
})
.WithOpenApi(operation => new(operation) {
    Tags = featureTags,
    Summary = "This result from service injected CNTXT.C",
});

var set = true;
app.MapGet("/fm", async (IFeatureManager fm, IConfiguration config, ILogger<Program> logger) =>
{
    var result = new List<KeyValuePair<string, string>>();
    await foreach (var key in fm.GetFeatureNamesAsync())
    {
        try {
            set = await fm.IsEnabledAsync(key);
            result.Add(new KeyValuePair<string, string>(key, (await fm.IsEnabledAsync(key)).ToString()));
        } catch (FeatureManagementException ex) {
            if (key == "TEST.KEYQ")
                logger.LogInformation(ex, $"Expected error getting key {key}");
            else {
                logger.LogError(ex, $"Error getting key {key}");
                throw;
            }
        }
    }

    const string keyD = "PLAIN.KEYD";
    if (!result.Any(o => o.Key == keyD)) { // will always error
        try {
            set = await fm.IsEnabledAsync(keyD);
        }
        catch (FeatureManagementException ex) {
            logger.LogInformation(ex, $"Expected error getting key keyD");
        }
    }

    // Test IConfiguration not found
    result.Add(new KeyValuePair<string, string>("WhatTimeIsItNot", config["WhatTimeIsItNot"] ?? "Not found in IConfiguration")); // never throws, just returns null

    return result.OrderBy(x => x.Key).ToArray();
})
.WithOpenApi(operation => new(operation) {
    Tags = featureTags,
    Summary = "Get the all the flags",
});


app.MapGet("/fm/context", async (IFeatureManager fm, IConfiguration config) =>
{
    var result = new List<KeyValuePair<string, string>>();
    var fc = new FeatureContext() { EnableMe = set };
    for (char x = 'A'; x < 'E'; x++)
    {
        try
        {
            result.Add(new KeyValuePair<string, string>($"Context CNTXT.KEY{x}", (await fm.IsEnabledAsync($"CNTXT.KEY{x}", fc)).ToString()));
        }
        catch (FeatureManagementException ex)
        {
            result.Add(new KeyValuePair<string, string>($"Context CNTXT.KEY{x}", $"Exception! {ex.Message}"));
        }
    }
    result.Add(new KeyValuePair<string, string>("Set is", set.ToString()));
    set = !set;

    return result.OrderBy(x => x.Key).ToArray();
})
.WithOpenApi(operation => new(operation) {
    Tags = featureTags,
    Summary = "Gets the flags with a Context",
});



app.MapGet("/fm/no-context", async (IFeatureManager fm, IConfiguration config) => {
    var result = new List<KeyValuePair<string, string>>();
    for (char x = 'A'; x < 'E'; x++) {
        try {
            result.Add(new KeyValuePair<string, string>($"No context CNTXT.KEY{x}", (await fm.IsEnabledAsync($"CNTXT.KEY{x}")).ToString()));
        }
        catch (FeatureManagementException ex) {
            result.Add(new KeyValuePair<string, string>($"No context CNTXT.KEY{x}", $"Exception! {ex.Message}"));
        }
    }
    return result.OrderBy(x => x.Key).ToArray();
})
.WithOpenApi(operation => new(operation) {
    Tags = featureTags,
    Summary = "This gets no context",
});

app.MapPost("/fm/{flagName}/set", (string flagName, IFeatureFlag ff) => {
    ff.Set(flagName, true);
})
.WithOpenApi(operation => new(operation) {
    Tags = featureTags,
    Summary = "Set a flag"
});

app.MapPost("/fm/{flagName}/clear", (string flagName, IFeatureFlag ff) => {
    ff.Set(flagName, false);
})
.WithOpenApi(operation => new(operation) {
    Tags = featureTags,
    Summary = "Set a flag"
});

app.MapGet("/fm/snapshot/{flagName}", (string flagName, IFeatureManagerSnapshot ff) => {
    return ff.IsEnabledAsync(flagName);
})
.WithOpenApi(operation => new(operation) {
    Tags = featureTags,
    Summary = "Get snapshot flag"
});

#endregion

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
