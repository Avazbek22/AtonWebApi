using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Application.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);

// 1. Добавляем DbContext с InMemory-провайдером
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseInMemoryDatabase("UserInMemoryDb");
});

// 2. Регистрируем репозиторий
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

// 3. Регистрируем MediatR (сканируем сборку Application)
builder.Services.AddMediatR(Assembly.Load("Application"));

// 4. Добавляем контроллеры
builder.Services.AddControllers();

// 5. Добавляем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---
// 6. Сидирование: создадим начального пользователя Admin
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    // Если ещё нет пользователя с логином "Admin", добавим его
    if (!db.Users.Any(u => u.Login == "Admin"))
    {
        db.Users.Add(new Domain.Entities.User
        {
            Guid = Guid.NewGuid(),
            Login = "Admin",
            Password = "Admin123", // пароль простейший
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
// ---

// 7. Конфигурируем pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Здесь можно добавить middleware, например, логирование, но для простоты не добавляем

app.UseAuthorization();

app.MapControllers();

app.Run();