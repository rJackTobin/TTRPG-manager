using Microsoft.Win32;
using System.Collections.ObjectModel;
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
            _config = manager.LoadConfig();
            this.WindowState = WindowState.Maximized;
            this.DataContext = _config;
            ApplyConfig();
            PopulateCharacterPanels();
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
                PopulateCharacterPanels();
            }
        }

        private void EditMode_Click(object sender, RoutedEventArgs e)
        {
            ShowEditDialog();
        }
        private void ShowEditDialog()
        {
            EditModeWindow editModeWindow = new EditModeWindow(this._config, this.Height, this.Width);
            var dialogResult = editModeWindow.ShowDialog();
            if (dialogResult == true)
            {
                _config = manager.LoadConfig();
                ApplyConfig();
            }
        }
        private void PartyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _config.selectedPartyIndex = partyComboBox.SelectedIndex;
            ConfigManager.SaveConfig(_config);
        }

        private void PopulateCharacterPanels()
        {
            CharacterPanels.Children.Clear(); // Clear existing panels if any

            foreach (Character character in _config.Parties[_config.selectedPartyIndex].Members)
            {
                var panel = new StackPanel
                {
                    Background = new SolidColorBrush(Colors.LightGray) { Opacity = 0.9 }, // Example styling
                    Margin = new Thickness(5), // Example styling
                    Width = Width/6, // Example sizing
                    Height = Height*29/30 // Example sizing
                };

                // Example content: Add a TextBlock for the character's name
                var characterNameText = new TextBlock
                {
                    Text = character.Name,
                    FontSize = Width / 100,
                    Foreground = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(10) // Just for some padding
                };
                Image characterImage = null;
                if (!string.IsNullOrEmpty(character.ImagePath))
                {
                    
                    characterImage = new Image
                    {
                        Source = new BitmapImage(new Uri(character.ImagePath, UriKind.Absolute)),
                        Width = Width/6, // Set the height or let it be auto-sized based on the image's aspect ratio
                        Margin = new Thickness(5) // Optional: Adjust the margin as needed
                    };
                     // Add the Image to the panel
                }


                panel.Children.Add(characterNameText); // Add the TextBlock to the panel
                if (characterImage != null) { panel.Children.Add(characterImage); }
                
                

                CharacterPanels.Children.Add(panel); // Finally, add the panel to the CharacterPanels stack panel
            }
        }
    }
}