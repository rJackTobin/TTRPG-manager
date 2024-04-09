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
using Genesis;
using System.Windows.Interop;
using System.Diagnostics;
using System.Windows.Media.Animation;


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
            SetImageSize();
            this.Loaded += MainWindow_Loaded;
            this.SizeChanged += MainWindow_SizeChanged;
            UpdateUI();

        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(WndProc);
        }

        private const int WM_DPICHANGED = 0x02E0;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DPICHANGED)
            {
                handled = true;
            }
            return IntPtr.Zero;
        }
        private void LoadCharAnim(Character character)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(character.ImagePath));
            var rect = new Int32Rect(0, 50, bitmap.PixelWidth, bitmap.PixelHeight - 100); // Cropping 50px from top and 50px from bottom
            CroppedBitmap croppedBitmap = new CroppedBitmap(bitmap, rect);

            imageBrush.ImageSource = croppedBitmap;
        }
        private void AnimateImage()
        {
            var canvasWidth = this.Width; // Ensure the canvas stretches across the window for this to work correctly
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = canvasWidth - animatedImage.Width; // Subtract width of the rectangle to prevent it from going off screen
            animation.Duration = TimeSpan.FromSeconds(2); // Duration of 2 seconds
            animation.RepeatBehavior = RepeatBehavior.Forever; // Repeat forever

            Storyboard.SetTarget(animation, animatedImage);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            serverManager.StopServer();
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonsWidth();
            LoadCharAnim(_config.Parties[_config.selectedPartyIndex].Members[0]);
            AnimateImage();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetButtonsWidth();
        }

        private void SetButtonsWidth()
        {
            double width = this.ActualWidth / 10;
            foreach (var child in mainPanel.Children)
            {
                if (child is Button)
                {
                    ((Button)child).Width = width;
                }
                else if (child is ComboBox)
                {
                    ((ComboBox)child).Width = width;
                }
            }
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
        public void SetImageSize()
        {
            double totalPanelWidth = CharacterPanels.Children.Count * ((Width / 7) + 10) + 15;

            // Set the maximum width of MainImage to take up the remaining space
            MainImage.MaxWidth = Width - totalPanelWidth;
            MainImage.MaxHeight = Height / 1.33;
            MainImage.Margin = new Thickness(0, 0, (Width - totalPanelWidth - MainImage.MaxWidth) / 2, 0);
        }
        private void MainImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateImageMargin();
        }

        private void UpdateImageMargin()
        {
            double totalPanelWidth = CharacterPanels.Children.Count * ((Width / 7) + 10) + 15;

            // Assuming MainImage.Source is not null and properly set
            double imageActualWidth = MainImage.ActualWidth;
            MainImage.MaxWidth = Width - totalPanelWidth;
            MainImage.MaxHeight = Height / 1.33;

            // Calculate the right margin
            double availableWidthForImage = Width - totalPanelWidth;
            double rightMargin = (availableWidthForImage - imageActualWidth + 8) / 2;

            MainImage.Margin = new Thickness(0, 0, rightMargin, 0);
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
                SetImageSize();
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
            if (!_config.usingNgrok)
            {
                if (serverManager.StartServer())
                {
                    try
                    {
                        serverManager.GetLocalIPAddress();
                        
                    }
                    catch (Exception ex)
                    {
                        txtIpAddress.Text = "Unable to determine IP address";
                    }
                }
                string localIP = serverManager.GetLocalIPAddress();
                txtIpAddress.Text = $"Connect to: http://{localIP}:8080";
            } else
            {
                string url = serverManager.StartOnlineServer();
                serverManager.StartServer();
                txtIpAddress.Text = $"Connect to: {url}";
            }

         }
        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            var imageSelectionWindow = new ImageSelectionWindow();
            imageSelectionWindow.ImageSelected += ImageSelectionWindow_ImageSelected;
            imageSelectionWindow.Closed += (s, args) => imageSelectionWindow.ImageSelected -= ImageSelectionWindow_ImageSelected;
            imageSelectionWindow.Show();
        }


        private void ImageSelectionWindow_ImageSelected(string imagePath)
        {
            MainImage.Source = new BitmapImage(new Uri(imagePath));
        }
        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return; // Safeguard, should not happen
            var character = button.Tag as Character;
            if (character == null) return; // Another safeguard
            characterManager.NewItem(character);
            ConfigManager.SaveConfig(_config);
            PopulateCharacterPanels();
        }
        private void AddSkillButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return; // Safeguard, should not happen
            var character = button.Tag as Character;
            if (character == null) return; // Another safeguard
            if (character == null) return; // Another safeguard
            characterManager.NewSkill(character);
            ConfigManager.SaveConfig(_config);
            PopulateCharacterPanels();
        }
        private void chooseItemButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void chooseSkillButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnAmbush_Click(object sender, RoutedEventArgs e)
        {
            CombatManager.Ambush();
            UpdateUI();
        }

        private void btnNextTurn_Click(object sender, RoutedEventArgs e)
        {
            CombatManager.NextTurn();
            UpdateUI();
        }

        private void btnEndCombat_Click(object sender, RoutedEventArgs e)
        {
            CombatManager.EndCombat();
            UpdateUI();
        }

        private void UpdateUI()
        {
            // Update the label to reflect the current turn
            lblTurnStatus.Content = $"Turn: {CombatManager.turn}";

            // Change label color based on who's turn it is
            if (CombatManager.isPlayersTurn)
            {
                lblTurnStatus.Foreground = new SolidColorBrush(Colors.MidnightBlue);
            }
            else if (CombatManager.isEnemiesTurn)
            {
                lblTurnStatus.Foreground = new SolidColorBrush(Colors.DarkRed);
            }
            else
            {
                lblTurnStatus.Foreground = new SolidColorBrush(Colors.MidnightBlue); // Default color when neither's turn
            }
        }
        private void PopulateCharacterPanels()
        {
            CharacterPanels.Children.Clear(); // Clear existing panels 
            if (_config.Parties.Count > 0 && _config.selectedPartyIndex >= 0 && _config.selectedPartyIndex < _config.Parties.Count)
            { 
                foreach (Character character in _config.Parties[_config.selectedPartyIndex].Members)
                {
                    var panel = new StackPanel
                    {
                        Background = new SolidColorBrush(Colors.LightGray) { Opacity = 0.9 }, // Example styling
                        Margin = new Thickness(5), // Example styling
                        Width = Width / 7, // Example sizing
                        Height = Height * 29 / 30, // Example sizing
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
                            Width = Width / 7, // Set the height or let it be auto-sized based on the image's aspect ratio
                            Margin = new Thickness(5) // Optional: Adjust the margin as needed
                        };
                        // Add the Image to the panel
                    }
                    // Horizontal StackPanel for Actions and Currency
                    var InfoPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5, 2, 5, 2)
                    };

                    var actionsText = new TextBlock
                    {
                        Text = $"Actions/Turn: {character.ActionsPerTurn} ",
                        FontSize = Width / 150,
                        Foreground = new SolidColorBrush(Colors.Black),
                        Margin = new Thickness(0)
                    };

                    var currencyText = new TextBlock
                    {
                        Text = $"Currency: {character.Currency} ",
                        FontSize = Width / 150,
                        Foreground = new SolidColorBrush(Colors.Black),
                        Margin = new Thickness(0)
                    };

                    InfoPanel.Children.Add(actionsText);
                    InfoPanel.Children.Add(currencyText);
                    var HPPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    var HPBar = new ProgressBar
                    {
                        Margin = new Thickness(0, 2, 0, 2),
                        Height = Height / 50,
                        Width = Width / 8,
                        Value = character.CurrentHP,
                        Maximum = character.MaxHP,
                        Foreground = new SolidColorBrush(Colors.Crimson),
                    };
                    var plusHP = new Button
                    {
                        BorderThickness = new Thickness(0),
                        Content = ">"
                    };
                    plusHP.Click += (sender, e) => { character.UpHP(1); ConfigManager.SaveConfig(_config); PopulateCharacterPanels(); };
                    var minusHP = new Button
                    {
                        BorderThickness = new Thickness(0),
                        Content = "<"
                    };
                    minusHP.Click += (sender, e) => { character.DownHP(1); ConfigManager.SaveConfig(_config); PopulateCharacterPanels(); };
                    HPPanel.Children.Add(minusHP);
                    HPPanel.Children.Add(HPBar);
                    HPPanel.Children.Add(plusHP);
                    var MPPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    var MPBar = new ProgressBar
                    {
                        Margin = new Thickness(0, 2, 0, 2),
                        Height = Height / 50,
                        Width = Width / 8,
                        Value = character.CurrentMP,
                        Maximum = character.MaxMP,
                        Foreground = new SolidColorBrush(Colors.CadetBlue),
                    };
                    var plusMP = new Button
                    {
                        BorderThickness = new Thickness(0),
                        Content = ">"
                    };
                    plusMP.Click += (sender, e) => { character.UpMP(1); ConfigManager.SaveConfig(_config); PopulateCharacterPanels(); };
                    var minusMP = new Button
                    {
                        BorderThickness = new Thickness(0),
                        Content = "<"
                    };
                    minusMP.Click += (sender, e) => { character.DownMP(1); ConfigManager.SaveConfig(_config); PopulateCharacterPanels(); };
                    MPPanel.Children.Add(minusMP);
                    MPPanel.Children.Add(MPBar);
                    MPPanel.Children.Add(plusMP);
                    var skillPanel = new StackPanel
                    {
                        Height = Height / 30,
                        Orientation = Orientation.Horizontal,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    // Populate skills
                    var skillsText = new TextBlock
                    {
                        Text = "Skills: ",
                        FontSize = Width / 150,
                        Foreground = new SolidColorBrush(Colors.Black),
                        Margin = new Thickness(Width / 500, 5, Width / 500, 5),
                        TextWrapping = TextWrapping.Wrap,
                        Width = Width / 24
                    };
                    var addSkillButton = new Button
                    {
                        Content = "New",
                        Tag = character, // Store the character in the Tag property
                        Margin = new Thickness(Width / 500, Height / 150, Width / 500, Height / 150),
                        Width = Width / 60,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        FontSize = Width / 150

                    };
                    addSkillButton.Click += AddSkillButton_Click;
                    var chooseSkillButton = new Button
                    {
                        Content = "+",
                        Tag = character, // Store the character in the Tag property
                        Margin = new Thickness(Width / 17, Height / 150, Width / 500, Height / 150),
                        Width = Width / 60,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        FontSize = Width / 150
                    };
                    chooseSkillButton.Click += chooseSkillButton_Click;
                    //adding elements to the skills panel
                    skillPanel.Children.Add(skillsText);
                    skillPanel.Children.Add(chooseSkillButton);
                    skillPanel.Children.Add(addSkillButton);

                    // Populate equipped items
                    var equippedItemsText = new TextBlock
                    {
                        Text = "Equipped: ",
                        FontSize = Width / 150,
                        Foreground = new SolidColorBrush(Colors.Black),
                        Margin = new Thickness(5),
                        TextWrapping = TextWrapping.Wrap,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Width = Width / 24
                    };
                    var inventoryPanel = new StackPanel
                    {
                        Height = Height / 30,
                        Orientation = Orientation.Horizontal,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    // Populate inventory
                    var inventoryText = new TextBlock
                    {
                        Text = "Inventory: ",
                        FontSize = Width / 150,
                        Foreground = new SolidColorBrush(Colors.Black),
                        Margin = new Thickness(Width / 500, 5, Width / 500, 5),
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = Width / 24
                    };
                    var addItemButton = new Button
                    {
                        Content = "New",
                        Tag = character, // Store the character in the Tag property
                        Width = Width / 60,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(Width / 500, Height / 150, Width / 500, Height / 150),

                        FontSize = Width / 150
                    };
                    addItemButton.Click += AddItemButton_Click;
                    var chooseItemButton = new Button
                    {
                        Content = "+",
                        Tag = character, // Store the character in the Tag property
                        Width = Width / 60,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(Width / 17, Height / 150, Width / 500, Height / 150),

                        FontSize = Width / 150
                    };
                    chooseItemButton.Click += chooseItemButton_Click;
                    //Adding elements to the inventory panel
                    inventoryPanel.Children.Add(inventoryText);
                    inventoryPanel.Children.Add(chooseItemButton);
                    inventoryPanel.Children.Add(addItemButton);

                    // Adding elements to the main panel
                    panel.Children.Add(characterNameText);
                    if (characterImage != null) panel.Children.Add(characterImage);
                    panel.Children.Add(InfoPanel);
                    panel.Children.Add(HPPanel);
                    panel.Children.Add(MPPanel);
                    panel.Children.Add(skillPanel);
                    foreach (var skill in character.Skills)
                    {
                        // Create the Expander
                        var expander = new Expander
                        {
                            Margin = new Thickness(5, 2, 5, 2),
                            Tag = skill // Store the item object in the Tag for easy access
                        };

                        // Create a TextBox for the item name in the Expander's Header
                        var nameTextBox = new TextBox
                        {
                            Text = skill.Name,
                            Margin = new Thickness(0),
                            BorderThickness = new Thickness(0), // Optionally remove border for a cleaner look
                            Background = new SolidColorBrush(Colors.Transparent), // Optional: make the TextBox background transparent
                            Width = Width / 11,
                        };
                        // Bind the TextChanged event to update the item's name
                        nameTextBox.TextChanged += (sender, e) =>
                        {
                            var textBox = sender as TextBox;
                            var currentItem = expander.Tag as Skill;
                            if (textBox != null && currentItem != null)
                            {
                                currentItem.Name = textBox.Text; // Update the item's name with the new text
                            }
                            ConfigManager.SaveConfig(_config);
                        };
                        var contextMenu = new ContextMenu
                        {

                        };

                        var deleteMenuItem = new MenuItem { Header = "Delete" };
                        {
                            deleteMenuItem.Click += (sender, e) => { character.RemoveSkill(skill); ConfigManager.SaveConfig(_config); PopulateCharacterPanels(); };
                        };
                        // Add menu items to the context menu

                        contextMenu.Items.Add(deleteMenuItem);

                        // Assign the context menu to the Expander
                        expander.ContextMenu = contextMenu;
                        // Set the Expander's header to the TextBox
                        expander.Header = nameTextBox;

                        // Create a TextBox for the item's description inside the Expander's content
                        var descriptionTextBox = new TextBox
                        {
                            Text = skill.Description,
                            IsReadOnly = false, // Or false if you want it to be editable
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Thickness(5),
                            AcceptsReturn = true,
                            Background = new SolidColorBrush(Colors.Transparent), // Optional
                            BorderThickness = new Thickness(0) // Optionally remove border
                        };
                        descriptionTextBox.TextChanged += (sender, e) =>
                        {
                            var textBox = sender as TextBox;
                            var currentSkill = expander.Tag as Skill;
                            if (textBox != null && currentSkill != null)
                            {
                                currentSkill.Description = textBox.Text; // Update the item's name with the new text
                            }
                            ConfigManager.SaveConfig(_config);
                        };
                        expander.Content = descriptionTextBox;

                        // Add the Expander to the panel
                        panel.Children.Add(expander);
                    }
                    panel.Children.Add(equippedItemsText);
                    foreach (var item in character.EquippedItems)
                    {
                        // Create the Expander
                        var expander = new Expander
                        {
                            Margin = new Thickness(5, 2, 5, 2),
                            Tag = item, // Store the item object in the Tag for easy access

                        };

                        // Create a TextBox for the item name in the Expander's Header
                        var nameTextBox = new TextBox
                        {
                            Text = item.Name,
                            Margin = new Thickness(0),
                            BorderThickness = new Thickness(0), // Optionally remove border for a cleaner look
                            Background = new SolidColorBrush(Colors.Transparent), // Optional: make the TextBox background transparent
                            Width = Width / 7,
                        };
                        // Bind the TextChanged event to update the item's name
                        nameTextBox.TextChanged += (sender, e) =>
                        {
                            var textBox = sender as TextBox;
                            var currentItem = expander.Tag as Item;
                            if (textBox != null && currentItem != null)
                            {
                                currentItem.Name = textBox.Text; // Update the item's name with the new text
                            }
                            ConfigManager.SaveConfig(_config);
                        };
                        var contextMenu = new ContextMenu
                        {

                        };

                        var deleteMenuItem = new MenuItem { Header = "Delete" };
                        {
                            deleteMenuItem.Click += (sender, e) => { character.RemoveItem(item); ConfigManager.SaveConfig(_config); PopulateCharacterPanels(); };
                        };

                        var unequipMenuItem = new MenuItem { Header = "Unequip" };
                        {
                            unequipMenuItem.Click += (sender, e) => { character.Unequip(item); ConfigManager.SaveConfig(_config); PopulateCharacterPanels(); };
                        };

                        // Add menu items to the context menu
                        contextMenu.Items.Add(unequipMenuItem);
                        contextMenu.Items.Add(deleteMenuItem);

                        // Assign the context menu to the Expander
                        expander.ContextMenu = contextMenu;
                        // Set the Expander's header to the TextBox
                        expander.Header = nameTextBox;
                        // Create a TextBox for the item's description inside the Expander's content
                        var descriptionTextBox = new TextBox
                        {
                            Text = item.Description,
                            IsReadOnly = false, // Or false if you want it to be editable
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Thickness(5),
                            AcceptsReturn = true,
                            Background = new SolidColorBrush(Colors.Transparent), // Optional
                            BorderThickness = new Thickness(0) // Optionally remove border
                        };
                        descriptionTextBox.TextChanged += (sender, e) =>
                        {
                            var textBox = sender as TextBox;
                            var currentItem = expander.Tag as Item;
                            if (textBox != null && currentItem != null)
                            {
                                currentItem.Description = textBox.Text; // Update the item's name with the new text
                            }
                            ConfigManager.SaveConfig(_config);
                        };
                        expander.Content = descriptionTextBox;

                        // Add the Expander to the panel
                        panel.Children.Add(expander);
                    }
                    panel.Children.Add(inventoryPanel);

                    foreach (var item in character.Inventory)
                    {

                        // Create the Expander
                        var expander = new Expander
                        {
                            Margin = new Thickness(5, 2, 5, 2),
                            Tag = item, // Store the item object in the Tag for easy access

                        };
                        // Create a TextBox for the item name in the Expander's Header
                        var nameTextBox = new TextBox
                        {
                            Text = item.Name,
                            Margin = new Thickness(0),
                            BorderThickness = new Thickness(0), // Optionally remove border for a cleaner look
                            Background = new SolidColorBrush(Colors.Transparent), // Optional: make the TextBox background transparent
                            Width = Width / 7,
                        };

                        // Bind the TextChanged event to update the item's name
                        nameTextBox.TextChanged += (sender, e) =>
                        {
                            var textBox = sender as TextBox;
                            var currentItem = expander.Tag as Item;
                            if (textBox != null && currentItem != null)
                            {
                                currentItem.Name = textBox.Text; // Update the item's name with the new text
                            }
                            ConfigManager.SaveConfig(_config);
                        };

                        // Set the Expander's header to the TextBox
                        expander.Header = nameTextBox;

                        var contextMenu = new ContextMenu
                        {

                        };

                        var deleteMenuItem = new MenuItem { Header = "Delete" };
                        {
                            deleteMenuItem.Click += (sender, e) => { character.RemoveItem(item); ConfigManager.SaveConfig(_config); PopulateCharacterPanels(); };
                        };

                        var equipMenuItem = new MenuItem { Header = "Equip" };
                        {
                            equipMenuItem.Click += (sender, e) => { character.Equip(item); ConfigManager.SaveConfig(_config); PopulateCharacterPanels(); };
                        };

                        // Add menu items to the context menu
                        contextMenu.Items.Add(equipMenuItem);
                        contextMenu.Items.Add(deleteMenuItem);

                        // Assign the context menu to the Expander
                        expander.ContextMenu = contextMenu;
                        // Create a TextBox for the item's description inside the Expander's content
                        var descriptionTextBox = new TextBox
                        {
                            Text = item.Description,
                            IsReadOnly = false, // Or false if you want it to be editable
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Thickness(5),
                            AcceptsReturn = true,
                            Background = new SolidColorBrush(Colors.LightGray), // Optional
                            BorderThickness = new Thickness(0) // Optionally remove border
                        };
                        descriptionTextBox.TextChanged += (sender, e) =>
                        {
                            var textBox = sender as TextBox;
                            var currentItem = expander.Tag as Item;
                            if (textBox != null && currentItem != null)
                            {
                                currentItem.Description = textBox.Text; // Update the item's name with the new text
                            }
                            ConfigManager.SaveConfig(_config);
                        };
                        expander.Content = descriptionTextBox;

                        // Add the Expander to the panel
                        panel.Children.Add(expander);

                    }
                    CharacterPanels.Children.Add(panel); // Finally, add the panel to the CharacterPanels stack panel
                }
            }
        }
        
    }
}