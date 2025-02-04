using System.Text;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Monitoring.Services;
using Monitoring.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Monitoring;
using Monitoring.Models.MonitoringModule;
using Monitoring.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load environment variables

builder.Services.AddControllers();

builder.Services.AddScoped<CheckerFactory>();


// Load environment variables
DotEnv.Load();

// Add services
var connectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"User={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

// ✅ Configure Database
builder.Services.AddDbContext<MonitoringDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 33)))
);

// ✅ Configure Identity with `Client` as ApplicationUser
builder.Services.AddIdentity<Client, IdentityRole>()
    .AddEntityFrameworkStores<MonitoringDbContext>()
    .AddDefaultTokenProviders();

// ✅ Load JWT Key from `.env` or `appsettings.json`
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") 
             ?? builder.Configuration["Jwt:Key"]; // Fallback to appsettings.json

// ✅ Configure Authentication (JWT Bearer)
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],  
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });


// ✅ Add Authorization Services
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// ✅ Add Swagger Configuration for JWT Authentication
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your JWT token}' in the field below"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//Add Email Sender Service
builder.Services.AddSingleton<IEmailSender<Client>, EmailSender>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// ✅ Register Services
builder.Services.AddScoped<CheckResultsRepository>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<MonitoringSystem>();
builder.Services.AddSingleton<Scheduler>(provider => 
    new Scheduler(provider)); 
builder.Services.AddSingleton<Scheduler>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<CheckResultsRepository>();
builder.Services.AddScoped<MonitoringService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedRoles(roleManager);
}

// ✅ Map Identity Routes using `Client` as ApplicationUser
app.MapIdentityApi<Client>();



// Configure the HTTP request pipeline.
// ✅ Configure Middleware Order
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();  // ✅ Ensure JWT Token Processing Before Authorization
app.UseAuthorization();
app.MapControllers();


// ✅ Run the Application
app.Run();

async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    string[] roles = { "Admin", "User", "Manager" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}