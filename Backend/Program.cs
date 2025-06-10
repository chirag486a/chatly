using System.Text;
using Chatly.Data;
using Chatly.DTO;
using Chatly.Interfaces.Services;
using Chatly.Models;
using Chatly.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            options.DefaultChallengeScheme =
                options.DefaultForbidScheme =
                    options.DefaultScheme =
                        options.DefaultSignInScheme =
                            options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
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

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Development Mode");
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();
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