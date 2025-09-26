using Application.Interfaces;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// framework services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// your own services
builder.Services.AddSingleton<IUserRepository, UserService>();
builder.Services.AddSingleton<IEventService, EventService>();
builder.Services.AddSingleton<IProductRepository, ProductService>();
builder.Services.AddSingleton<IRedemptionService, RedemptionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
