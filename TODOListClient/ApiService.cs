using System.Net.Http;
using System.Net.Http.Json;
using TODOListClient.Models;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7196/");
    }

    // Получить категории
    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Category>>("api/categories");
    }

    // Добавить категорию
    public async Task AddCategoryAsync(Category category)
    {
        await _httpClient.PostAsJsonAsync("api/categories", category);
    }

    // Получить задачи
    public async Task<List<TaskItem>> GetTasksAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TaskItem>>("api/tasks");
    }

    // Добавить задачу
    public async Task AddTaskAsync(TaskItem task)
    {
        await _httpClient.PostAsJsonAsync("api/tasks", task);
    }

    // Обновить задачу
    public async Task UpdateTaskAsync(int id, TaskItem updatedTask)
    {
        await _httpClient.PutAsJsonAsync($"api/tasks/{id}", updatedTask);
    }

    // Удалить задачу
    public async Task DeleteTaskAsync(int id)
    {
        await _httpClient.DeleteAsync($"api/tasks/{id}");
    }
}