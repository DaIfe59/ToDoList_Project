using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TODOListServer.Data;
using TODOListServer.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Настройка сериализации JSON (System.Text.Json)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // используем имена как в атрибутах JsonPropertyName
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Настройка подключения к базе данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=TODOListRemastered_BaseData.db"));

// Настройка Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoList API", Version = "v1" });
});

var app = builder.Build();

// Инициализация базы данных
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    
    if (!context.Categories.Any())
    {
        var categories = new List<Category>
        {
            new Category { Name = "Работа" },
            new Category { Name = "Личное" },
            new Category { Name = "Покупки" },
            new Category { Name = "Здоровье" }
        };
        context.Categories.AddRange(categories);
        context.SaveChanges();
        Console.WriteLine("Добавлены тестовые категории");
    }
}

// Включаем Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoList API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.MapControllers();
app.Run();