using Config.Application.Commands;
using Config.Application.DTOs;
using Config.Application.Queries;
using Config.Persistence;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<ListByAppQuery>());

// Mongo conn: appsettings veya env
var mongoConn = builder.Configuration["Mongo:ConnectionString"]
                ?? Environment.GetEnvironmentVariable("Mongo__ConnectionString")
                ?? "mongodb://mongo:27017";

builder.Services.AddPersistence(mongoConn);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// GET: tüm kayýtlar
app.MapGet("/api/configs", async (string? app, string? q, IMediator m, CancellationToken ct) =>
    Results.Ok(await m.Send(new ListByAppQuery(app, q), ct))
);

// POST: upsert
app.MapPost("/api/configs", async (ConfigDto dto, IMediator m) =>
{
    await m.Send(new UpsertConfigCommand(dto));
    return Results.Ok();
});

// DELETE: sil
app.MapDelete("/api/configs/{appName}/{name}", async (string appName, string name, IMediator m) =>
{
    await m.Send(new DeleteConfigCommand(appName, name));
    return Results.Ok();
});

app.Run();
