using Api.Server.Mappings;
using Application.Interfaces;
using Application.Services;
using AutoMapper;                        
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// framework services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<RewardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RewardDb")));

//// Register In-Memory Repositories for Milestone 1
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

// Register Application Services
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRedemptionService, RedemptionService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

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

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

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

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
