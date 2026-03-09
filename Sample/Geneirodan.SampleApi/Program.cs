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