using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Api.Configuration;
using Api.Configuration.Databases;
using Api.Configuration.Services;
using Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var shelteredDatabaseSettings = builder.Configuration.GetRequiredSettings<ShelteredDatabaseSettings>();
builder.Services.AddDatabase<ShelteredContext>(shelteredDatabaseSettings);
builder.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.EnsureDatabaseCreatedAsync<ShelteredContext>();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// The main program for running the API.
/// </summary>
/// <remarks>
/// Added to allow applying the <see cref="ExcludeFromCodeCoverageAttribute"/> to this file.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed partial class Program { }
