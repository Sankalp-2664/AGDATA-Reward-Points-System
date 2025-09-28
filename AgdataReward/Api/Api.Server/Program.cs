using Application.Interfaces;
using Application.Services;
using Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// framework services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ?? Register In-Memory Repositories for Milestone 1
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IUserAccountRepository, InMemoryUserAccountRepository>();
builder.Services.AddSingleton<IEventDefinitionRepository, InMemoryEventDefinitionRepository>();
builder.Services.AddSingleton<IEventRewardRuleRepository, InMemoryEventRewardRuleRepository>();
builder.Services.AddSingleton<IEventInstanceRepository, InMemoryEventInstanceRepository>();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<IProductInventoryRepository, InMemoryProductInventoryRepository>();
builder.Services.AddSingleton<IRewardPointsRepository, InMemoryRewardPointsRepository>();
builder.Services.AddSingleton<IRewardTransactionRepository, InMemoryRewardTransactionRepository>();
builder.Services.AddSingleton<IRedemptionRecordRepository, InMemoryRedemptionRecordRepository>();
builder.Services.AddSingleton<IRedemptionProcessRepository, InMemoryRedemptionProcessRepository>();

// ?? Register Application Services
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IRedemptionService, RedemptionService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

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

// Run milestone-1 demo before starting the web server
await Api.Server.Milestone1.Milestone1Program.Run();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
