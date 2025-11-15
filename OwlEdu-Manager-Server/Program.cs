using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// BẮT BUỘC: Lắng nghe trên PORT của Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8000";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// Lấy config từ biến môi trường (Railway inject)
var sqlServerConnection = builder.Configuration.GetConnectionString("SQLSERVERDB_CONNECTION")
                          ?? Environment.GetEnvironmentVariable("SQLSERVERDB_CONNECTION");

var jwtKey = builder.Configuration["JWT_SECRET"]
             ?? Environment.GetEnvironmentVariable("JWT_SECRET");

// Add services
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddDbContext<EnglishCenterManagementContext>(options =>
    options.UseSqlServer(sqlServerConnection));

builder.Services.AddAuthorization();
builder.Services.AddApplicationServices();
builder.Services.AddSingleton(new JwtService(jwtKey));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Migrate + Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EnglishCenterManagementContext>();
    try
    {
        db.Database.Migrate();
        await SeedData.InitializeAsync(db);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check
app.MapGet("/health", () => "OK");

app.Run();