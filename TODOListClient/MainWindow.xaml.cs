using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TODOListClient.Models;

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
                cmbCategory.ItemsSource = categories;
            }
            catch (Exception ex)
            {
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
                foreach (var task in tasks)
                {
                    listBoxTasks.Items.Add($"{task.Title} ({task.Category?.Name ?? "Без категории"})");
                }
            }
            catch (Exception ex)
            {
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
            var task = new TaskItem
            {
                Title = txtTask.Text,
                CategoryId = category.Id
            };

            try
            {
                await _apiService.AddTaskAsync(task);
                LoadData(); // Обновляем список задач
                txtTask.Clear(); // Очищаем поле ввода
            }
            catch (Exception ex)
            {
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