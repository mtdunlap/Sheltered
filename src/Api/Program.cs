using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Api.Animals;
using Api.Configuration.Databases;
using Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var databaseOptions = builder.Configuration.GetDatabaseSettings();
builder.Services.AddDatabase<ShelteredContext>(databaseOptions);
builder.Services.AddScoped<IRepository<ShelteredContext>, Repository<ShelteredContext>>();
builder.Services.AddScoped<IAnimalMapper, AnimalMapper>();

var app = builder.Build();
await app.EnsureDeletedAndMigrateAsync<ShelteredContext>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
