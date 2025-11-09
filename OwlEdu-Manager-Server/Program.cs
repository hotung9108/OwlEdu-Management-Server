using DotNetEnv;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
DotNetEnv.Env.Load();
var sqlServerConnection = Environment.GetEnvironmentVariable("SQLSERVERDB_CONNECTION");
var oracleConnection = Environment.GetEnvironmentVariable("ORACLEDB_CONNECTION");

builder.Services.AddScoped(sp => new SqlConnection(sqlServerConnection));
//builder.Services.AddScoped(sp => new OracleConnection(oracleConnection));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
DotNetEnv.Env.Load();
//Environment.GetEnvironmentVariable("")
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
