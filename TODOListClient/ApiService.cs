using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using TODOListClient.Models;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5143/");
    }

    // Получить категории
    public async Task<List<Category>> GetCategoriesAsync()
    {
        var response = await _httpClient.GetAsync("api/Categories");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<List<Category>>(JsonOptions);
        return content ?? new List<Category>();
    }

    // Добавить категорию
    public async Task AddCategoryAsync(Category category)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Categories", category, JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    // Получить задачи
    public async Task<List<TaskItem>> GetTasksAsync()
    {
        var response = await _httpClient.GetAsync("api/Tasks");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<List<TaskItem>>(JsonOptions);
        return content ?? new List<TaskItem>();
    }

    // Добавить задачу
    public async Task AddTaskAsync(TaskItem task)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Tasks", task, JsonOptions);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"HTTP {response.StatusCode}: {errorContent}");
        }
    }

    // Обновить задачу
    public async Task UpdateTaskAsync(int id, TaskItem updatedTask)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Tasks/{id}", updatedTask, JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    // Удалить задачу
    public async Task DeleteTaskAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/Tasks/{id}");
        response.EnsureSuccessStatusCode();
    }
}