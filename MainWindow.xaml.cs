using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TTRPG_manager
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private AppConfig _config;
        
        ConfigManager manager = new ConfigManager();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            
            _config = manager.LoadConfig();
            ApplyConfig();
        }

        private void ApplyConfig()
        {
            var parts = _config.Resolution.Split('x');
            this.Width = int.Parse(parts[0]);
            this.Height = int.Parse(parts[1]);
            if (_config.BackgroundPath != "")
            {
                try
                {
                    this.background.ImageSource = new BitmapImage(new Uri(_config.BackgroundPath, UriKind.RelativeOrAbsolute));
                }
                catch (Exception ex)
                {
                    //do nothing
                }
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowSettingsDialog();
        }
        private void ShowSettingsDialog()
        {
            SettingsWindow settingsWindow = new SettingsWindow(this._config, this.Height, this.Width);
            var dialogResult = settingsWindow.ShowDialog();
            if (dialogResult == true)
            {
                _config = manager.LoadConfig();
                ApplyConfig();
            }
        }

    }
}