using System. Text;
using Microsoft.AspNetCore. Authentication.JwtBearer;
using Microsoft.AspNetCore. Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft. IdentityModel. Tokens;
using Microsoft.OpenApi.Models;
using FinFlowAPI.API.Services;
using FinFlowAPI.Data;
using FinFlowAPI.Middleware;
using FinFlowAPI.Services;
using FinFlowAPI.Services.Auth;
using FinFlowAPI.Services.Comments;
using FinFlowAPI.Services.Interactions;
using FinFlowAPI.Services.Posts;
using FinFlowAPI.Services.Role;
using FinFlowAPI.Services.Transactions;
using FinFlowAPI.Services.User;

var builder = WebApplication.CreateBuilder(args);

builder.Services. AddControllers();
builder. Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header, Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});



// Database
builder.Services. AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services. AddScoped<IRoleService, RoleService>();
builder. Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IPostsService, PostsService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IInteractionService, InteractionService>();
builder.Services.AddScoped<CommonService>();
builder.Services.AddHttpClient("NepseApi", client =>
    {
        client.BaseAddress = new Uri("https://nepseapi.surajrimal.dev/");
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AllowAutoRedirect = true,
        MaxAutomaticRedirections = 5
    });
// Authorization handlers
builder. Services.AddScoped<IAuthorizationHandler, FunctionHandler>();
builder.Services.AddScoped<IAuthorizationHandler, PrivilegeHandler>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder. Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]! ))
        };
    });

// Dynamic Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Function-based policies
    var functionCodes = new[]
    {
        "PREVIEW_DASHBOARD", "FILTER_DATE", "DOWNLOAD",
        "ADD_STAFF", "EDIT_STAFF", "DELETE_STAFF",
        "ADD_STUDENT", "EDIT_STUDENT", "DELETE_STUDENT",
        "ADD_LOCATION", "EDIT_LOCATION", "DELETE_LOCATION"
    };

    foreach (var code in functionCodes)
    {
        options.AddPolicy($"Function_{code}", policy =>
            policy.Requirements.Add(new FunctionRequirement(code)));
    }

    // Privilege-based policies
    var privilegeCodes = new[]
    {
        "VIEW_STATISTICS", "VIEW_ENROLLMENT_ANALYTICS", "VIEW_APPLICATION_OVERVIEW", "VIEW_INTAKES_STATUS",
        "VIEW_STAFF_LIST", "VIEW_STAFF_DETAILS", "VIEW_TEACHER_REPORT",
        "VIEW_STUDENT_LIST", "VIEW_STUDENT_DETAILS", "VIEW_STUDENT_REPORT",
        "VIEW_LOCATION_LIST", "VIEW_LOCATION_DETAILS", "VIEW_ROLES"
    };

    foreach (var code in privilegeCodes)
    {
        options.AddPolicy($"Privilege_{code}", policy =>
            policy.Requirements.Add(new PrivilegeRequirement(code)));
    }
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy. WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              . AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment. IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



app.Run();