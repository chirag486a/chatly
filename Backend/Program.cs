using System.Text;
using Chatly.Data;
using Chatly.DTO;
using Chatly.Hubs;
using Chatly.Interfaces.Repositories;
using Chatly.Interfaces.Services;
using Chatly.Models;
using Chatly.Repositories;
using Chatly.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWTKey is required.");
}

var encodedKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];


if (string.IsNullOrEmpty(jwtIssuer) && string.IsNullOrEmpty(jwtAudience))
{
    throw new Exception("Jwt configuration settings is missing or invalid.");
}

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


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
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = encodedKey
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for SignalR hub path
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/contactHub")))
                {
                    // Read the token out of query string
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPasswordFormatValidator, PasswordFormatValidator>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});


var app = builder.Build();


// Get all endpoints
var apiDescriptionProvider = app.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
var apiDescriptions = apiDescriptionProvider.ApiDescriptionGroups.Items;

foreach (var group in apiDescriptions)
{
    foreach (var apiDescription in group.Items)
    {
        Console.WriteLine($"Path: {apiDescription.RelativePath}");
        Console.WriteLine($"Method: {apiDescription.HttpMethod}");
        Console.WriteLine($"Parameters: {string.Join(", ", apiDescription.ParameterDescriptions.Select(p => p.Name))}");
        Console.WriteLine("---");
    }
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<ContactHub>("/contactHub");

app.MapFallback(() => Results.NotFound(ApiResponse<object>.ErrorResponse(
    "404 Not Found",
    400,
    "NOT_IMPLEMENTED",
    "The route that you are trying to access is not implemented.",
    new Dictionary<string, List<string>>
    {
        { "Server", new List<string> { "The path is invalid" } }
    }
)));
app.Run();