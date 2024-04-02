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
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;

namespace TTRPG_manager
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private AppConfig updated_config;

        public SettingsWindow(double height, double width)
        {
            InitializeComponent();

            updated_config = ConfigManager.LoadConfig();
            // Or however you instantiate it

            // Copy values from old_config to new_config if needed

            this.DataContext = updated_config; // Set DataContext for data binding
            var parts = updated_config.Resolution.Split('x');
            this.Width = int.Parse(parts[0])*0.5;
            this.Height = int.Parse(parts[1])*0.5;
            
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.SaveConfig(updated_config);
            this.DialogResult = true;
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private async void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png", // Filters to only include image files
                Title = "Select an Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                updated_config.BackgroundPath = (openFileDialog.FileName);
                this.FilePath.Text = openFileDialog.FileName;
            }
        }
    }
}
