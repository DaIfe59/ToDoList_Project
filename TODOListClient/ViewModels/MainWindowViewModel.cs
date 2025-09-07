using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TODOListClient.Models;

namespace TODOListClient.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ApiService api = new ApiService(new HttpClient());

        public ObservableCollection<Category> Categories { get; } = new();
        public ObservableCollection<TaskItem> Tasks { get; } = new();

        [ObservableProperty]
        private string taskTitle = string.Empty;

        [ObservableProperty]
        private Category? selectedCategory;

        [ObservableProperty]
        private TaskItem? selectedTask;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        public MainWindowViewModel()
        {
            _ = LoadInitialAsync();
        }

        private async Task LoadInitialAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            StatusMessage = "Загрузка данных...";
            
            try
            {
                await LoadCategoriesAsync();
                await LoadTasksAsync();
                StatusMessage = "Данные загружены успешно";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке данных: {ex.Message}";
                StatusMessage = "Ошибка загрузки";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadCategoriesAsync()
        {
            var cats = await api.GetCategoriesAsync();
            Categories.Clear();
            foreach (var c in cats) Categories.Add(c);
            if (Categories.Any()) SelectedCategory = Categories.First();
        }

        private async Task LoadTasksAsync()
        {
            var items = await api.GetTasksAsync();
            Tasks.Clear();
            foreach (var t in items) Tasks.Add(t);
        }

        [RelayCommand]
        private async Task AddTaskAsync()
        {
            if (string.IsNullOrWhiteSpace(TaskTitle) || SelectedCategory == null) 
            {
                ErrorMessage = "Введите задачу и выберите категорию!";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;
            StatusMessage = "Добавление задачи...";

            try
            {
                var newTask = new TaskItem { Title = TaskTitle.Trim(), CategoryId = SelectedCategory.Id };
                await api.AddTaskAsync(newTask);
                await LoadTasksAsync(); // Обновляем список задач
                TaskTitle = string.Empty;
                StatusMessage = "Задача добавлена успешно";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при добавлении задачи: {ex.Message}";
                StatusMessage = "Ошибка добавления";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteTaskAsync()
        {
            if (SelectedTask == null)
            {
                ErrorMessage = "Выберите задачу для удаления!";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;
            StatusMessage = "Удаление задачи...";

            try
            {
                await api.DeleteTaskAsync(SelectedTask.Id);
                await LoadTasksAsync(); // Обновляем список задач
                StatusMessage = "Задача удалена успешно";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при удалении задачи: {ex.Message}";
                StatusMessage = "Ошибка удаления";
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        // "Добавить категорию"
        [RelayCommand]
        private async Task AddCategoryAsync()
        {
            // Создаем диалог для ввода названия категории
            var inputDialog = new TextInputDialog("Введите название категории:")
            {
                Owner = Application.Current.MainWindow
            };

            if (inputDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputDialog.ResponseText))
            {
                try
                {
                    var newCategory = new Category { Name = inputDialog.ResponseText.Trim() };
                    await api.AddCategoryAsync(newCategory);
                    await LoadCategoriesAsync(); // Обновляем список категорий
                    await LoadTasksAsync(); // Обновляем список задач
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // "Удалить категорию"
        [RelayCommand]
        private async Task DeleteCategoryAsync()
        {
            if (SelectedCategory == null)
            {
                MessageBox.Show("Выберите категорию для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await api.DeleteCategoryAsync(SelectedCategory.Id);
                await LoadCategoriesAsync(); // Обновляем список категорий
                await LoadTasksAsync(); // Обновляем список задач
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}