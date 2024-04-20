using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TTRPG_manager
{
    /// <summary>
    /// Interaction logic for SelectionWindow.xaml
    /// </summary>
    public partial class SelectionWindow : Window
    {
        public INameable SelectedItem { get; private set; }

        private IEnumerable<INameable> items;

        public SelectionWindow(IEnumerable<INameable> items, double height, double width)
        {
            InitializeComponent();
            this.items = items;
            listBox.ItemsSource = items;
            
            this.Height = height / 3;
            this.Width = width / 5;
            listBox.Height = this.Height*3/4;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            listBox.ItemsSource = string.IsNullOrWhiteSpace(searchBox.Text) ?
                items :
                items.Where(item => item.Name.Contains(searchBox.Text, StringComparison.OrdinalIgnoreCase));
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = listBox.SelectedItem as INameable;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                this.DialogResult = true;
            }
                this.Close();
        }
    }
}
