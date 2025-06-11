using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Application.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);

builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseInMemoryDatabase("UserInMemoryDb");
});

builder.Services.AddScoped<IUserRepository, EfUserRepository>();

builder.Services.AddMediatR(Assembly.Load("Application"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    
    if (!db.Users.Any(u => u.Login == "Admin"))
    {
        db.Users.Add(new Domain.Entities.User
        {
            Guid = Guid.NewGuid(),
            Login = "Admin",
            Password = "Admin123", 
            Name = "Administrator",
            Gender = 2,
            Birthday = null,
            Admin = true,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "System"
        });
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();