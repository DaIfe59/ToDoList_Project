using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TODOListClient.Models;
using System.Linq; // Added for .Any() and .Select()

namespace TODOListClient
{
    public partial class MainWindow : Window
    {
        private readonly ApiService _apiService;

        public MainWindow()
        {
            InitializeComponent();
            var httpClient = new HttpClient();
            _apiService = new ApiService(httpClient);

            // Загружаем данные при запуске
            LoadCategories();
            LoadData();
        }

        // Загрузка категорий из сервера
        private async void LoadCategories()
        {
            try
            {
                var categories = await _apiService.GetCategoriesAsync();
                Console.WriteLine($"Загружено {categories.Count} категорий: {string.Join(", ", categories.Select(c => $"{c.Id}:{c.Name}"))}");
                cmbCategory.ItemsSource = categories;
                
                // Выбираем первую категорию по умолчанию
                if (categories.Any())
                {
                    cmbCategory.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке категорий: {ex.Message}");
                MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Загрузка задач из сервера
        private async void LoadData()
        {
            try
            {
                listBoxTasks.Items.Clear();
                var tasks = await _apiService.GetTasksAsync();
                Console.WriteLine($"Загружено {tasks.Count} задач");
                
                foreach (var task in tasks)
                {
                    var categoryName = task.Category?.Name ?? "Без категории";
                    Console.WriteLine($"Задача: {task.Title}, CategoryId: {task.CategoryId}, Category: {categoryName}");
                    listBoxTasks.Items.Add($"{task.Title} ({categoryName})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
                MessageBox.Show($"Ошибка при загрузке задач: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик нажатия кнопки "Добавить"
        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTask.Text) || cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Введите задачу и выберите категорию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var category = cmbCategory.SelectedItem as Category;
            Console.WriteLine($"Выбрана категория: ID={category.Id}, Name={category.Name}");
            
            var task = new TaskItem
            {
                Title = txtTask.Text,
                CategoryId = category.Id
            };
            
            Console.WriteLine($"Создаем задачу: Title={task.Title}, CategoryId={task.CategoryId}");
            
            try
            {
                await _apiService.AddTaskAsync(task);
                Console.WriteLine("Задача успешно добавлена");
                LoadData(); // Обновляем список задач
                txtTask.Clear(); // Очищаем поле ввода
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении задачи: {ex.Message}");
                MessageBox.Show($"Ошибка при добавлении задачи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик нажатия клавиши Enter в текстовом поле
        private void txtTask_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnAdd_Click(sender, e);
            }
        }

        // Обработчик нажатия кнопки "Удалить"
        private async void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxTasks.SelectedItem == null)
            {
                MessageBox.Show("Выберите задачу для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string selectedItem = listBoxTasks.SelectedItem.ToString();
                string taskTitle = selectedItem.Split('(')[0].Trim();

                // Находим ID задачи по её названию
                var tasks = await _apiService.GetTasksAsync();
                var taskToRemove = tasks.Find(t => t.Title == taskTitle);

                if (taskToRemove != null)
                {
                    await _apiService.DeleteTaskAsync(taskToRemove.Id);
                    LoadData(); // Обновляем список задач
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении задачи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик нажатия клавиши Delete в списке задач
        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                btnRemove_Click(sender, e);
            }
        }
    }
}