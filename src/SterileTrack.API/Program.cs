using Microsoft.EntityFrameworkCore;
using SterileTrack.Application.Mappings;
using SterileTrack.Application.Services;
using SterileTrack.Domain.Interfaces;
using SterileTrack.Infrastructure.Data;
using SterileTrack.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SterileTrack API",
        Version = "v1",
        Description = "Device Inventory Tracking System API"
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure Entity Framework with MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Server=localhost;Database=SterileTrack;User=root;Password=rootpassword;Port=3306;";

builder.Services.AddDbContext<SterileTrackDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableStringComparisonTranslations()));

// Register repositories and unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IStatusHistoryRepository, StatusHistoryRepository>();
builder.Services.AddScoped<ISterilizationCycleRepository, SterilizationCycleRepository>();

// Register application services
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<ISterilizationCycleService, SterilizationCycleService>();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SterileTrackDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
