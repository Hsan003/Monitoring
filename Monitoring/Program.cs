using dotenv.net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monitoring.Services;
using Monitoring;
using Monitoring.Models.MonitoringModule;
using Monitoring.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services
    .AddScoped<CheckerFactory>()
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<MonitoringDbContext>();


// Load environment variables
DotEnv.Load();

// Add services
var connectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"User={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

builder.Services.AddDbContext<MonitoringDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Default"),
ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))
    ));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<MonitoringSystem>();
builder.Services.AddSingleton<Scheduler>(provider => 
    new Scheduler(provider)); 
builder.Services.AddSingleton<Scheduler>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<CheckResultsRepository>();
builder.Services.AddScoped<MonitoringService>();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ENDPOINT MAPPING
app.MapIdentityApi<IdentityUser>();
app.MapControllers(); // Critical for controller routes

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();




app.Run();
