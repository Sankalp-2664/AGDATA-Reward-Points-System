using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence.Repositories;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// framework services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddSingleton<IUserRepository, UserService>();
//builder.Services.AddSingleton<IEventService, EventService>();
//builder.Services.AddSingleton<IProductRepository, ProductService>();
//builder.Services.AddSingleton<IRedemptionService, RedemptionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


await Api.Server.Milestone1.Milestone1Program.Run();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


