using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using ToDo.Authorization.Handlers;
using ToDo.Authorization.Requirements;
using ToDo.Data.Entities;
using ToDo.Extensions;
using ToDo.Features.Logins.Services;
using ToDo.Features.ToDos;
using ToDo.Features.ToDos.DTO;
using ToDo.Features.ToDos.Mappings;
using ToDo.Features.ToDos.Services;
using ToDo.Features.Users.Services;
using ToDo.Helpers.Emails;
using ToDo.Helpers.OTPs;
using ToDo.Helpers.Tokens;
var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Users\EnaNguyen\AppData\Roaming\ToDo"))
    .SetApplicationName("ToDoApp");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Token validated. User roles: {Roles}",
                context.Principal?.Claims.Where(c => c.Type == ClaimTypes.Role)
                                      .Select(c => c.Value));
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SameUsernamePolicy", policy =>
    {
        policy.RequireAuthenticatedUser();           
        policy.AddRequirements(new SameUsernameRequirement());
    });
});
builder.Services.AddScoped<IAuthorizationHandler, SameUsernameAuthorizationHandler>();
builder.Services.AddScoped<IToDoServices, ToDoServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IOTPServices, OTPServices>();
builder.Services.AddScoped<IEmailServices, EmailServices>();
builder.Services.AddScoped<ITokenServices, TokenServices>();
builder.Services.AddScoped<IAuthenticationServices, AuthenticationServices>();
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
app.UseAuthentication();   
app.UseAuthorization();

app.Run();

