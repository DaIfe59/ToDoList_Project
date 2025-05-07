using Microsoft.EntityFrameworkCore;
using TODOListServer.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавление контекста базы данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=TODOListRemastered_BaseData.db"));

// Добавление контроллеров
builder.Services.AddControllers();

var app = builder.Build();

// Настройка маршрутизации
app.MapControllers();

app.Run();