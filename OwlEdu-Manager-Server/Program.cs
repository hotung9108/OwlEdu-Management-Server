using DotNetEnv;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Environment.GetEnvironmentVariable("")
DotNetEnv.Env.Load();
var sqlServerConnection = Environment.GetEnvironmentVariable("SQLSERVERDB_CONNECTION");

builder.Services.AddDbContext<EnglishCenterManagementContext>(options =>
options.UseSqlServer(sqlServerConnection));
//var oracleConnection = Environment.GetEnvironmentVariable("ORACLEDB_CONNECTION");

//builder.Services.AddScoped(sp => new SqlConnection(sqlServerConnection));
//builder.Services.AddScoped(sp => new OracleConnection(oracleConnection));
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(sqlServerConnection));
//builder.Services.AddScoped<AccountService>();
//builder.Services.AddScoped<StudentService>();
builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
