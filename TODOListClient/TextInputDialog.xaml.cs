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
    /// Логика взаимодействия для TextInputDialog.xaml
    /// </summary>
    public partial class TextInputDialog : Window
    {
        private readonly ApiService _apiService;
        public string ResponseText {  get; private set; }
        public TextInputDialog(string promt)
        {
            InitializeComponent();
            Title = promt;
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ResponseText = txtInputNameCategory.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
