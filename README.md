# ToDo List Application (.NET 9, WPF + ASP.NET Core)

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-5C2D91)
![WPF](https://img.shields.io/badge/WPF-Desktop-4B8BBE)
![SQLite](https://img.shields.io/badge/SQLite-DB-003B57?logo=sqlite&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green)

## Описание
Простое приложение «Список дел» с архитектурой клиент–сервер:
- Сервер — REST API на ASP.NET Core 9 + Entity Framework Core + SQLite
- Клиент — WPF (.NET 9)
- Поддержка категорий и задач, привязка задач к категориям
- JSON-сериализация через System.Text.Json

## Подключение к базе данных (SQLite)
Ниже — максимально простая инструкция, чтобы быстро запустить проект даже если вы видите его впервые.

### Вариант A — ничего не настраивать (по умолчанию)
- База данных — обычный файл SQLite.
- При первом запуске сервер сам создаст файл БД, если его нет.
- Путь и имя файла: `TODOListRemastered_BaseData.db` (лежит рядом с сервером).
- Просто выполните:
```
cd ToDoList_Remastered/TODOListServer
 dotnet restore
 dotnet run
```
- Откройте браузер: `http://localhost:5143` (Swagger UI появится на главной странице).
- После этого запустите клиент:
```
cd ToDoList_Remastered/TODOListClient
 dotnet restore
 dotnet run
```

### Вариант B — указать свой путь к БД
Если вы хотите хранить БД в другом месте или под другим именем, поменяйте строку подключения.

Где менять:
1) `TODOListServer/Program.cs` — конфигурация DI:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=ПУТЬ_К_БАЗЕ/ИмяБД.db"));
```
2) `TODOListServer/Data/AppDbContext.cs` — запасная конфигурация контекста:
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlite("Data Source=ПУТЬ_К_БАЗЕ/ИмяБД.db");
}
```
Совет: пропишите одинаковый путь в обоих местах. Пример:
```csharp
"Data Source=C:/Data/ToDoList.db"
```

### Если БД уже существует
- Ничего дополнительно делать не нужно — сервер подключится к существующему файлу.
- Если структура БД устарела, примените миграции EF Core либо удалите файл БД, чтобы сервер пересоздал её с актуальной схемой.

Применить миграции (опционально, если у вас установлен dotnet-ef):
```
cd ToDoList_Remastered/TODOListServer
 dotnet tool restore
 dotnet ef database update
```