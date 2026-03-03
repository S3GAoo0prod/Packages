/// <summary>
/// Sample API entry point: registers MediatR pipeline (with current assembly), FluentValidation validators, Serilog, and OpenTelemetry.
/// Maps a minimal root endpoint; add more endpoints that send commands/queries and use <see cref="ResultConverter.MapResult"/> for responses.
/// </summary>

using System.Reflection;
using FluentValidation;
using Geneirodan.MediatR;
using Geneirodan.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatRPipeline(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.AddSerilog();
builder.Services.AddSharedOpenTelemetry(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();