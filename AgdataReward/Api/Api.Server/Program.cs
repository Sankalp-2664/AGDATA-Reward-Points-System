using Application.Interfaces;
using Application.Services;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

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


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reward API v1");
        c.RoutePrefix = string.Empty;  // opens directly at http://localhost:5106/
    });
}

// Run milestone-1 demo 
//await Api.Server.Milestone1.Milestone1Program.Run();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
