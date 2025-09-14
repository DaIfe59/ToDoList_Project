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
            var dialog = new RemoveCategoryDialog(api)
            {
                Owner = Application.Current.MainWindow
            };

            var ok = dialog.ShowDialog() == true;
            if (!ok) return; // Отмена или неуспех — просто выходим

            IsLoading = true;
            ErrorMessage = string.Empty;
            StatusMessage = "Обновление данных...";

            try
            {
                await LoadCategoriesAsync();
                await LoadTasksAsync();
                StatusMessage = "Категория удалена";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при обновлении данных: {ex.Message}";
                StatusMessage = "Ошибка";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // "Изменить задачу"
        [RelayCommand]
        private async Task ChangeTaskAsync()
        {
            var dialog = new TaskEditDialog()
            {
                Owner = Application.Current.MainWindow
            };
            if (SelectedTask == null)
            {
                ErrorMessage = "Выберите задачу для изменения!";
                return;
            }
            IsLoading = true;
            ErrorMessage = string.Empty;
            StatusMessage = "Изменение задачи...";
            if (SelectedTask != null && dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResponseText)&& dialog.DialogResult == true)
            {
                try
                {
                    SelectedTask.Title = dialog.ResponseText;
                    await api.UpdateTaskAsync(SelectedTask.Id, SelectedTask);
                    await LoadTasksAsync();
                    StatusMessage = "Задача изменена успешно";
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Ошибка при изменение задачи: {ex.Message}";
                    StatusMessage = "Ошибка добавления";
                }
                finally { IsLoading = false; }
            }
            if (dialog.DialogResult == false)
            {
                StatusMessage = string.Empty;
                IsLoading = false;
            }
        }
    }
}