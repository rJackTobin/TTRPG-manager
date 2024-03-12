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

namespace TTRPG_manager
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private AppConfig _config;
        public string SelectedResolution { get; private set; }
        public SettingsWindow(AppConfig config, double height, double width)
        {
            InitializeComponent();
            _config = config;
            this.Height = height * 0.5;
            this.Width = width * 0.5;
        }
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedResolution = (ResolutionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (!string.IsNullOrEmpty(selectedResolution))
            {
                _config.SelectedResolution = selectedResolution;
                
            }
            //await ConfigManager.SaveConfigAsync(_config);
        }

    }
}
