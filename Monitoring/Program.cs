using dotenv.net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monitoring.Services;
using Monitoring;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();


// Load environment variables
DotEnv.Load();

// Add services
var connectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"User={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

builder.Services.AddDbContext<MonitoringDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 33)))
);

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<MonitoringDbContext>();
builder.Services.AddControllers(); // <-- Add this line

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<CheckResultsRepository>();
builder.Services.AddScoped<AnalyticsService>();


var app = builder.Build();

//Map Identity Routes
app.MapIdentityApi<IdentityUser>();

//Middleware
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

 app.MapControllers(); 



app.Run();
