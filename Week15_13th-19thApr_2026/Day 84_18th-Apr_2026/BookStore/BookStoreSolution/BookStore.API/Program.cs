using BookStore.API.Extensions;
using BookStore.API.Middleware;
using BookStore.Application;
using BookStore.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day).CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfig();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApiVersioningConfig();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddCors(options => { options.AddPolicy("AllowMvc", policy => policy.WithOrigins("https://localhost:5002").AllowAnyHeader().AllowAnyMethod()); });

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStore API v1"); options.SwaggerEndpoint("/swagger/v2/swagger.json", "BookStore API v2"); });
app.UseCors("AllowMvc");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }