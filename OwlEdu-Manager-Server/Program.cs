using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Oracle.ManagedDataAccess.Client;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Environment.GetEnvironmentVariable("")
DotNetEnv.Env.Load();
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
string envFile = environment == "Development" ? ".env.development" : ".env.docker";

if (File.Exists(envFile))
{
    DotNetEnv.Env.Load(envFile);
}

var sqlServerConnection = Environment.GetEnvironmentVariable("SQLSERVERDB_CONNECTION");
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET");
//var sqlServerConnection = Environment.GetEnvironmentVariable("SQLSERVERDB_CONNECTION");
//var oracleConnection = Environment.GetEnvironmentVariable("ORACLEDB_CONNECTION");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(token) && !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Request.Headers["Authorization"] = $"Bearer {token}";
            }
            return Task.CompletedTask;
        }
    };
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
builder.Services.AddDbContext<EnglishCenterManagementContext>(options =>
    options.UseSqlServer(sqlServerConnection));
//builder.Services.AddDbContext<EnglishCenterManagementContext>(options =>
//        options.UseOracle(oracleConnection));

//builder.Services.AddScoped(sp => new SqlConnection(sqlServerConnection));
//builder.Services.AddScoped(sp => new OracleConnection(oracleConnection));
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(sqlServerConnection));
//builder.Services.AddScoped<AccountService>();
//builder.Services.AddScoped<StudentService>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
builder.Services.AddApplicationServices();
builder.Services.AddSingleton(new JwtService(jwtKey));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        //Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
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
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EnglishCenterManagementContext>();
    db.Database.Migrate();
    await SeedData.InitializeAsync(db);
}
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAuthorization();
app.MapControllers();
app.Run();
