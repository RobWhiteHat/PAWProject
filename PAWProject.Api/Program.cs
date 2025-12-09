using Microsoft.EntityFrameworkCore;
using PAW3CP1.Architecture;
using PAWProject.Api.Services.Contracts;
using PAWProject.Architecture.Services;
using PAWProject.Core;
using PAWProject.Core.Interfaces;
using PAWProject.Data;
using PAWProject.Data.Models;
using PAWProject.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<Pawg3Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISourceItemService, SourceItemService>();
builder.Services.AddScoped<ISourceService, SourceService>();
builder.Services.AddScoped<ISpaceService, SpaceService>();
builder.Services.AddScoped<IRepositorySource, RepositorySource>();
builder.Services.AddScoped<IRestProvider, RestProvider>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
