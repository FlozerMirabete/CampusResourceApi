using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Campus Resource API",
        Version = "v1",
        Description = "A simple API that lists sample campus resources."
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var resources = new List<CampusResource>
{
    new(1, "Campus Library", "Library", "Main Building", "Open weekdays, 8:00 AM–5:00 PM"),
    new(2, "Computer Laboratory", "Laboratory", "Science Building, Room 204", "Open weekdays, 9:00 AM–4:00 PM"),
    new(3, "Student Services Office", "Office", "Administration Building", "Open weekdays, 8:00 AM–5:00 PM")
};

app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapGet("/api/resources", () => Results.Ok(resources))
    .WithName("GetResources")
    .WithSummary("Get all campus resources")
    .Produces<List<CampusResource>>(StatusCodes.Status200OK);

app.MapGet("/api/resources/{id:int}", (int id) =>
{
    if (id <= 0)
    {
        return Results.BadRequest(new ErrorResponse(
            "invalid_id",
            "Resource ID must be a positive integer."));
    }

    var resource = resources.FirstOrDefault(item => item.Id == id);

    return resource is null
        ? Results.NotFound(new ErrorResponse(
            "resource_not_found",
            $"No campus resource was found with ID {id}."))
        : Results.Ok(resource);
})
    .WithName("GetResourceById")
    .WithSummary("Get one campus resource by ID")
    .Produces<CampusResource>(StatusCodes.Status200OK)
    .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
    .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

app.Run();

public record CampusResource(
    int Id,
    string Name,
    string Type,
    string Location,
    string Hours);

public record ErrorResponse(
    string Code,
    string Message);
