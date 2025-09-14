using System.Windows;
using System.Windows.Input;
using TODOListClient.ViewModels;

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
            DataContext = new RemoveCategoryDialogViewModel();
        }

        public RemoveCategoryDialog(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            DataContext = new RemoveCategoryDialogViewModel(_apiService);
        }

        // Обработчик нажатия "Enter" или "Delete"
        private async void CatListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter || e.Key == Key.Delete) && !(CatListBox.SelectedItem == null))
            {
                ConfirmButton_Click(sender, e);
            }
            if (e.Key == Key.Escape) CancelButton_Click(sender,e);
        }

        //Обработчик кнопки "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Обработчик кнопки "Подтвердить": дожидаемся выполнения команды VM и только затем закрываем
        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RemoveCategoryDialogViewModel vm)
            {
                if (vm.DeleteCategoryCommand is CommunityToolkit.Mvvm.Input.IAsyncRelayCommand asyncCmd)
                {
                    await asyncCmd.ExecuteAsync(null);
                }
                else
                {
                    vm.DeleteCategoryCommand.Execute(null);
                }
            }
            DialogResult = true;
            Close();
        }
    }
}
