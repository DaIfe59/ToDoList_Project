using Microsoft.EntityFrameworkCore;
using TODOListServer.Data;

var builder = WebApplication.CreateBuilder(args);

// ���������� ��������� ���� ������
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=TODOListRemastered_BaseData.db"));

// ���������� ������������
builder.Services.AddControllers();

var app = builder.Build();

// ��������� �������������
app.MapControllers();

app.Run();