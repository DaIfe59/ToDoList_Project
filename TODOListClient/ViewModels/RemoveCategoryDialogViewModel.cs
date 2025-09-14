using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TODOListClient.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace TODOListClient.ViewModels
{
    public partial class RemoveCategoryDialogViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        public ObservableCollection<Category> Categories { get; }

        [ObservableProperty]
        private string taskTitle = string.Empty;

        [ObservableProperty]
        private Category selectedCategory;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        public RemoveCategoryDialogViewModel()
        {
            _apiService = new ApiService(new HttpClient());
            Categories = new ObservableCollection<Category>();
            _ = LoadInitialAsync();
        }

        public RemoveCategoryDialogViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Categories = new ObservableCollection<Category>();
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
                StatusMessage = "Загрузка прошла успешно";
            }
            catch(Exception ex) {
                ErrorMessage = $"Ошибка при загрузке данных: {ex.Message}";
                StatusMessage = "Ошибка загрузки";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Метод загрузки категорий
        private async Task LoadCategoriesAsync()
        {
            var category = await _apiService.GetCategoriesAsync();
            if (category == null) return;
            Categories.Clear();
            foreach (var item in category) Categories.Add(item);
            if (Categories.Any()) SelectedCategory = Categories.First();
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
                await _apiService.DeleteCategoryAsync(SelectedCategory.Id);
                await LoadCategoriesAsync(); // Обновляем список категорий
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
