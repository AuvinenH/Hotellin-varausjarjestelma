using FluentValidation;
using FluentValidation.AspNetCore;
using HotelLakeview.Api.Middleware;
using HotelLakeview.Application;
using HotelLakeview.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
	options.AddPolicy("FrontendCors", policy =>
	{
		if (allowedOrigins.Length == 0)
		{
			policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
			return;
		}

		policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
	});
});
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(HotelLakeview.Application.DependencyInjection).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.Services.EnsureDatabaseCreatedAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("FrontendCors");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program;
