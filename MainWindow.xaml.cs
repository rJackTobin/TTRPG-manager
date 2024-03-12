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
        private int _resX = 1280;
        public int ResX
        {
            get => _resX;
            set
            {
                _resX = value;
                OnPropertyChanged(nameof(ResX));
            }
        }

        private int _resY = 720;
        public int ResY
        {
            get => _resY;
            set
            {
                _resY = value;
                OnPropertyChanged(nameof(ResY));
            }
        }
        private AppConfig _config;


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += async (sender, args) => await LoadConfig();
            // Explicitly set window size after initialization
            this.Width = ResX;
            this.Height = ResY;
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
                // Optionally, reload config or UI elements if settings could affect the main window
            }
        }
        private async Task LoadConfig()
        {
            _config = await ConfigManager.LoadConfigAsync();
            
        }

    }
}