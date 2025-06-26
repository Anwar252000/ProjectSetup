using Backend.Application.Interfaces;
using Backend.Application.Mappings;
using Backend.Application.Services;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.Database;
using Backend.Infrastructure.Repositories;
using LinqKit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "" };
        allowedOrigins.ForEach(allowedOrigin =>
        {
            Console.WriteLine($"AllowedOrigin: {allowedOrigin}");
        });

        policy.WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Add services to the container.

builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IJwtToken, JwtTokenService>();

builder.Services.AddScoped<IUserRoles, UserRoleService>();
builder.Services.AddScoped<IUsers, UserService>();
builder.Services.AddScoped<IUserPermissions, UserPermissionService>();
builder.Services.AddScoped<IVehicleDetails, VehicleDetailService>();

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Access Configuration from the builder
var configuration = builder.Configuration;

// JWT Authention
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Seed the database with default data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
    SeedDefaultData(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

void SeedDefaultData(DataBaseContext context)
{
    // Seed default roles and users

    if (!context.UserRoles.Any())
    {

        var defaultRole = new UserRole
        {
            RoleName = "Showroom Owner",
            CreatedAt = DateTime.Now,
            IsActive = true,
        };
        context.UserRoles.Add(defaultRole);
        context.SaveChanges();
    }

    var ownerRoleId = context.UserRoles
        .Where(r => r.RoleName == "Showroom Owner")
        .Select(r => r.UserRoleId)
        .FirstOrDefault();

    if (!context.Users.Any())
    {
        var password = "pak@12345";
        var hasher = new PasswordHasher<User>();
        var defaultUser = new User
        {
            UserName = "admin",
            FullName = "Pak Showroom",
            Email = "pakshowroom@example.com",
            Password = hasher.HashPassword(null, password),
            UserRoleId = ownerRoleId,
            IsActive = true,
            CreatedAt = DateTime.Now,
        };
        context.Users.Add(defaultUser);
        context.SaveChanges();
    }

    if (!context.UserPermissions.Any())
    {
        var userId = context.Users.Select(c => c.UserId).FirstOrDefault();
        context.UserPermissions.AddRange(new[]
        {
            new UserPermission
            {
                UserId = userId,
                PageUrl = "/",
                CreatedBy = userId,
                IsActive = true,
                CreatedAt = DateTime.Now,
            },
            new UserPermission
            {
                UserId = userId,
                PageUrl = "/dashboard",
                CreatedBy = userId,
                IsActive = true,
                CreatedAt = DateTime.Now,
            }
        });
        context.SaveChanges();
    }

}
