using Api.Server.Mappings;
using Api.Server.Services;
using Application.Configuration;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------
// 1. Framework services
// --------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Reward API v1",
        Version = "v1"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});


// --------------------------------------------------------
// 2. DbContext
// --------------------------------------------------------
builder.Services.AddDbContext<RewardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RewardDb")));


//angular 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",   // Angular default
                "http://localhost:51552",  // Alternative port
                "http://127.0.0.1:4200"    // Also allow 127.0.0.1
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// --------------------------------------------------------
// 3. Repository registrations
// --------------------------------------------------------

// In-memory repos (Milestone 1)
//builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
//builder.Services.AddSingleton<IUserAccountRepository, InMemoryUserAccountRepository>();
//builder.Services.AddSingleton<IEventDefinitionRepository, InMemoryEventDefinitionRepository>();
//builder.Services.AddSingleton<IEventRewardRuleRepository, InMemoryEventRewardRuleRepository>();
//builder.Services.AddSingleton<IEventInstanceRepository, InMemoryEventInstanceRepository>();
//builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
//builder.Services.AddSingleton<IProductInventoryRepository, InMemoryProductInventoryRepository>();
//builder.Services.AddSingleton<IRewardPointsRepository, InMemoryRewardPointsRepository>();
//builder.Services.AddSingleton<IRewardTransactionRepository, InMemoryRewardTransactionRepository>();
//builder.Services.AddSingleton<IRedemptionRecordRepository, InMemoryRedemptionRecordRepository>();
//builder.Services.AddSingleton<IRedemptionRequestRepository, InMemoryRedemptionRequestRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddScoped<IEventDefinitionRepository, EventDefinitionRepository>();
builder.Services.AddScoped<IEventRewardRuleRepository, EventRewardRuleRepository>();
builder.Services.AddScoped<IEventInstanceRepository, EventInstanceRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductInventoryRepository, ProductInventoryRepository>();
builder.Services.AddScoped<IRewardPointsRepository, RewardPointsRepository>();
builder.Services.AddScoped<IRewardTransactionRepository, RewardTransactionRepository>();
builder.Services.AddScoped<IRedemptionRecordRepository, RedemptionRecordRepository>();
builder.Services.AddScoped<IRedemptionRequestRepository, RedemptionRequestRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();


// --------------------------------------------------------
// 4. Application services
// --------------------------------------------------------
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRedemptionService, RedemptionService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<IRedemptionApiService, RedemptionApiService>();
builder.Services.AddScoped<IProductApiService, ProductApiService>();
builder.Services.AddScoped<IInventoryApiService, InventoryApiService>();
builder.Services.AddScoped<IEventApiService, EventApiService>();
builder.Services.AddScoped<IRewardApiService, RewardApiService>();


builder.Services.AddAutoMapper(typeof(MappingProfile));

// --------------------------------------------------------
// 5. JWT settings + authentication
// --------------------------------------------------------

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<JwtSettings>>().Value);

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt configuration is missing.");

// Clear default mapping so 'sub', 'role', etc. remain as-is
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services
    .AddAuthentication(options =>
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

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

builder.Services.AddAuthorization();

// --------------------------------------------------------
// 6. Build app
// --------------------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reward API v1");
        c.RoutePrefix = "swagger";
    });
}

// app.UseHttpsRedirection(); // enable when you want HTTPS

app.UseRouting();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply pending migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RewardDbContext>();
    await dbContext.Database.MigrateAsync();
    Console.WriteLine("✓ Database migrations applied.");
}

await DbSeeder.SeedAsync(app.Services);

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Application error: {ex}");
}
