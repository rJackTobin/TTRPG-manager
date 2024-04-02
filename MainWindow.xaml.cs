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
using System.IO;
using System.Threading;
using System;
using System.Net;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private AppConfig _config;
        
        

        private ServerManager serverManager = new ServerManager();

        public MainWindow()
        {
            InitializeComponent();
            _config = ConfigManager.LoadConfig();
            this.WindowState = WindowState.Maximized;
            this.DataContext = _config;
            ApplyConfig();
            PopulateCharacterPanels();
            
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            serverManager.StopServer();
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
            switch (_config.ScreenType)
            {
                case "Borderless Windowed":
                    this.WindowStyle = WindowStyle.None;
                    this.ResizeMode = ResizeMode.NoResize;
                    this.Topmost = false; // Ensure window stays on top
                    this.Left = 0; // Align window to the left edge of the screen
                    this.Top = 0; // Align window to the top edge of the screen

                    // Set the window size to the full screen size, not just the work area
                    this.Width = SystemParameters.PrimaryScreenWidth;
                    this.Height = SystemParameters.PrimaryScreenHeight;

                    this.WindowState = WindowState.Normal; // Use Normal state to apply custom size
                    break;

                case "Windowed":
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.ResizeMode = ResizeMode.CanResize;
                    // You might want to reset the WindowState or adjust MaxHeight and MaxWidth if needed
                    break;

                default:
                    // Handle unexpected screen type
                    break;
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
            SettingsWindow settingsWindow = new SettingsWindow(this.Height, this.Width);
            var dialogResult = settingsWindow.ShowDialog();
            if (dialogResult == true)
            {
                _config = ConfigManager.LoadConfig();
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
            EditModeWindow editModeWindow = new EditModeWindow(this.Height, this.Width);
            var dialogResult = editModeWindow.ShowDialog();
            if (dialogResult == true)
            {
                _config = ConfigManager.LoadConfig();
                ApplyConfig();
                PopulateCharacterPanels();
            }
        }
        private void PartyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _config = ConfigManager.LoadConfig();
            _config.selectedPartyIndex = partyComboBox.SelectedIndex;
            ConfigManager.SaveConfig(_config);
            PopulateCharacterPanels();
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            if (serverManager.StartServer())
            {
                try
                {
                    string localIP = serverManager.GetLocalIPAddress();
                    txtIpAddress.Text = $"Connect to: http://{localIP}:8080";
                }
                catch (Exception ex)
                {
                    txtIpAddress.Text = "Unable to determine IP address";
                }
            }
            
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
                // Horizontal StackPanel for Actions and Currency
                var InfoPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5)
                };

                var actionsText = new TextBlock
                {
                    Text = $"Actions/Turn: {character.ActionsPerTurn}",
                    FontSize = Width / 150,
                    Foreground = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(5)
                };

                var currencyText = new TextBlock
                {
                    Text = $"Currency: {character.Currency}",
                    FontSize = Width / 150,
                    Foreground = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(5)
                };

                InfoPanel.Children.Add(actionsText);
                InfoPanel.Children.Add(currencyText);

                // Populate skills
                var skillsText = new TextBlock
                {
                    Text = "Skills: " + string.Join(", ", character.Skills.Select(s => s.Name)),
                    FontSize = Width / 150,
                    Foreground = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(5),
                    TextWrapping = TextWrapping.Wrap
                };

                // Populate equipped items
                var equippedItemsText = new TextBlock
                {
                    Text = "Equipped: " + string.Join(", ", character.EquippedItems.Select(i => i.Name)),
                    FontSize = Width / 150,
                    Foreground = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(5),
                    TextWrapping = TextWrapping.Wrap
                };

                // Populate inventory
                var inventoryText = new TextBlock
                {
                    Text = "Inventory: " + string.Join(", ", character.Inventory.Select(i => i.Name)),
                    FontSize = Width / 150,
                    Foreground = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(5),
                    TextWrapping = TextWrapping.Wrap
                };

                // Adding elements to the main panel
                panel.Children.Add(characterNameText);
                if (characterImage != null) panel.Children.Add(characterImage);
                panel.Children.Add(InfoPanel);
                panel.Children.Add(skillsText);
                panel.Children.Add(equippedItemsText);
                panel.Children.Add(inventoryText);



                CharacterPanels.Children.Add(panel); // Finally, add the panel to the CharacterPanels stack panel
            }
        }
        
    }
}