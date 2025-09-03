using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TODOListClient
{
    /// <summary>
    /// Логика взаимодействия для RemoveCategoryDialog.xaml
    /// </summary>
    public partial class RemoveCategoryDialog : Window
    {
        private readonly ApiService _apiService;

        public RemoveCategoryDialog()
        {
            InitializeComponent();
            var httpClient = new HttpClient();
            _apiService = new ApiService(httpClient);

            LoadCategories();
        }

        // Обработчик нажатия "Enter" или "Delete"
        private async void CatListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter || e.Key == Key.Delete) && !(CatListBox.SelectedItem == null))
            {
                await RemoveCategoryAsync();
            }
        }

        //Обработчик кнопки "ОК"
        private async void ConfrimButton_Click(object sender, RoutedEventArgs e)
        {
            var success = await RemoveCategoryAsync();
            if (success)
            {
                DialogResult = true;
                Close();
            }
        }

        //Обработчик кнопки "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        //Метод для удаления категории
        private async Task<bool> RemoveCategoryAsync()
        {
            if (CatListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию для удаления!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            try
            {
                var selectedCategory = CatListBox.SelectedItem as Models.Category;
                if (selectedCategory != null)
                {
                    await _apiService.DeleteCategoryAsync(selectedCategory.Id);
                    await Task.Delay(50); // небольшой промежуток, чтобы сервер завершил транзакцию
                    LoadCategories();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        //
        private async void LoadCategories()
        {
            try
            {
                var categories = await _apiService.GetCategoriesAsync();
                Console.WriteLine($"Загружено {categories.Count} категорий: {string.Join(", ", categories.Select(c => $"{c.Id}:{c.Name}"))}");
                CatListBox.ItemsSource = categories;

                // Выбираем первую категорию по умолчанию
                if (categories.Any())
                {
                    CatListBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке категорий: {ex.Message}");
                MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
