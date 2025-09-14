using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для TaskEditDialog.xaml
    /// </summary>
    public partial class TaskEditDialog : Window
    {
        public string ResponseText { get; private set; }
        public TaskEditDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtInput3.Text))
            {
                ResponseText = txtInput3.Text;
                DialogResult = true;
            }
            else DialogResult = false;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void txtInput3_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) OkButton_Click((object)sender, e);
            if(e.Key == Key.Escape) CancelButton_Click((object)sender,e);
        }
    }
}
