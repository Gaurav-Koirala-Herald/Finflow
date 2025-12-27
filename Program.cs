using System.Text;
using Microsoft. AspNetCore.Authentication.JwtBearer;
using Microsoft. EntityFrameworkCore;
using Microsoft. IdentityModel. Tokens;
using Microsoft.OpenApi.Models;
using FinflowAPI.Data;
using FinflowAPI.DbImplementation;
using FinflowAPI.DbImplementation.Interfaces;
using FinflowAPI.Models;
using FinflowAPI.Services;
using FinflowAPI.Services.Auth;
using FinflowAPI.Services. Implementations;
using FinflowAPI.Services.Transaction;
using FinflowAPI.Services.UserManagement;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options. UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Settings
builder.Services. Configure<JwtSettings>(builder.Configuration. GetSection("JwtSettings"));

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.issuer,
        ValidAudience = jwtSettings. audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.jwtSecret)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();


// Register Services
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
// builder.Services. AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// Add Controllers
builder.Services. AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Finflow API",
        Version = "v1",
        Description = "Finance Tracker API"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType. Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
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
            Array.Empty<string>()
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure Pipeline
if (app. Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app. UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    // var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // dbContext.Database.Migrate();
    
    // Seed admin user
    // await DbSeeder.SeedAdminUserAsync(dbContext);
}

app.Run();