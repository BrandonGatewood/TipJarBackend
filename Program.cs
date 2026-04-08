using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TipJar.Api.Middleware;
using TipJar.Application.Services;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Domain.Interfaces.Security;
using TipJar.Domain.Interfaces.Services;
using TipJar.Infrastructure.Data;
using TipJar.Infrastructure.Repositories;
using TipJar.Infrastructure.Security;


builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

if (builder.Environment.IsDevelopment())
    Env.Load();

builder.Configuration.AddEnvironmentVariables();

// Jwt Configurations
var jwtIssuer = builder.Configuration["JWT:ISSUER"] 
    ?? throw new InvalidOperationException("JWT issuer is missing in configuration.");
var jwtAudience = builder.Configuration["JWT:AUDIENCE"] 
    ?? throw new InvalidOperationException("JWT audience is missing in configuration.");
var jwtKey = builder.Configuration["JWT:KEY"]
    ?? throw new InvalidOperationException("JWT key is missing in configuration.");

// Db Configuration
var connectionString = builder.Configuration["DB:CONNECTIONSTRING"]
    ?? throw new InvalidOperationException("Database connection string is missing in configuration.");
var redisConnectionString = builder.Configuration["REDIS:CONNECTIONSTRING"]
    ?? throw new InvalidOperationException("Redis connection string is missing in configuration.");

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITipRepository, TipRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITipService, TipService>();
builder.Services.AddStackExchangeRedisCache(Options =>
{
    Options.Configuration = redisConnectionString; 
    Options.InstanceName = "TipJar_";
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(3);
    });
});
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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        )
    };
});
var allowedOrigins = builder.Configuration["FRONTEND:URL"]?.Split(',') 
                     ?? ["http://localhost:5173"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowReactApp");
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();
app.Run();

