using Auth_API;
using Auth_API.Data;
using Auth_API.Models.Domain;
using Auth_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ContextSeedService>();

builder.Services.AddIdentityCore<User>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;

        options.SignIn.RequireConfirmedEmail = true;
    }
)
.AddRoles<IdentityRole>()
.AddRoleManager<RoleManager<IdentityRole>>()
.AddEntityFrameworkStores<UserDbContext>()
.AddSignInManager<SignInManager<User>>()
.AddUserManager<UserManager<User>>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = false,
        };
    });

builder.Services.AddCors();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
        .Where(x => x.Value.Errors.Count > 0)
        .SelectMany(x => x.Value.Errors)
        .Select(x => x.ErrorMessage).ToArray();

        var toReturn = new
        {
            Errors = errors
        };

        return new BadRequestObjectResult(toReturn);
    };
});

builder.Services.AddAuthorization(option => 
    {
        option.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
        option.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
        option.AddPolicy("PlayerPolicy", policy => policy.RequireRole("Player"));

        option.AddPolicy("AdminOrManagerPolicy", policy => policy.RequireRole("Admin", "Manager"));
        option.AddPolicy("AdminAndManagerPolicy", policy => policy.RequireRole("Admin").RequireRole("Manager"));
        option.AddPolicy("AllRolePolicy", policy => policy.RequireRole("Admin", "Manager", "Player"));

        option.AddPolicy("AdminEmailPolicy", policy => policy.RequireClaim(ClaimTypes.Email, "admin@example.com"));
        option.AddPolicy("MillerSurnamePolicy", policy => policy.RequireClaim(ClaimTypes.Surname, "miller"));

        option.AddPolicy("ManagerEmailAndWilsonSurnamePolicy", policy => policy
            .RequireClaim(ClaimTypes.Surname, "wilson")
            .RequireClaim(ClaimTypes.Email, "manager@example.com"));
        option.AddPolicy("VIPPolicy", policy => policy.RequireAssertion(context => SD.VIPPolicy(context)));
    });

var app = builder.Build();

app.UseCors(options =>
{
    options.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["JWT:ClientUrl"]);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

#region
using var scope = app.Services.CreateScope();
try
{
    var contextSeedService = scope.ServiceProvider.GetService<ContextSeedService>();
    await contextSeedService.InitializeContextAsync();
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
    logger.LogError(ex.Message, "Failed to initialize and seed the database");
}
#endregion

app.Run();
