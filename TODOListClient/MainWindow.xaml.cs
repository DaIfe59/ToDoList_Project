using System.Windows;
using System.Windows.Input;
using TODOListClient.ViewModels;

namespace TODOListClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Устанавливаем DataContext для MVVM
            DataContext = new MainWindowViewModel();
        }

        // Обработчик нажатия клавиши Enter в TextBox
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var viewModel = DataContext as MainWindowViewModel;
                viewModel?.AddTaskCommand.Execute(null);
            }
        }

        // Обработчик нажатия клавиши Delete в списке задач
        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var viewModel = DataContext as MainWindowViewModel;
                viewModel?.DeleteTaskCommand.Execute(null);
            }
        }
    }
}