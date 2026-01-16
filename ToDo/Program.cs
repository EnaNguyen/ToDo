using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ToDo.Data.Entities;
using FluentValidation;
using ToDo.Features.ToDos.DTO;
using ToDo.Features.ToDos;
using ToDo.Features.ToDos.Services;
using ToDo.Features.ToDos.Mappings;
using ToDo.Extensions;
using ToDo.Features.Users.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IToDoServices, ToDoServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(config => { }
, typeof(Program).Assembly
);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
var app = builder.Build();
app.MapEndpoints();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}
app.UseHttpsRedirection();

app.Run();

