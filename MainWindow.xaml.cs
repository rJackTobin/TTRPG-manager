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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Windows.Threading;
using Microsoft.VisualBasic.ApplicationServices;
using System.Windows.Media;


namespace TTRPG_manager
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private AppConfig _config;

        private ServerManager serverManager = new ServerManager();
        public static MainWindow Instance { get; private set; }
        Random rand = new Random();
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
            Instance = this;
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
        public void PlaceSticker(Sticker sticker)
        {
            if (!_config.StickersEnabled) { return; }
            MediaPlayer mediaPlayer = new MediaPlayer();
            if (sticker.MediaPath != null)
            {
                
                mediaPlayer.Open(new Uri(sticker.MediaPath, UriKind.RelativeOrAbsolute));
            }
            var image = new Image();
            image.Source = new BitmapImage(new Uri(sticker.ImagePath, UriKind.Absolute));
            image.Width = Width/10;  // Set the desired width for the image
            image.Height = Width/10; // Set the desired height for the image

            // Create an ellipse to act as the border
            var ellipse = new Ellipse();
            ellipse.Width = image.Width + 10; // Adding 10 for the border width
            ellipse.Height = image.Height + 10; // Adding 10 for the border height
            ellipse.Stroke = Brushes.White; // Set the border color to white
            ellipse.StrokeThickness = 5; // Set the thickness of the border
            ellipse.Fill = new ImageBrush(image.Source); // Fill the ellipse with the image

            // Apply a scale transform
            ScaleTransform scaleTransform = new ScaleTransform();
            ellipse.RenderTransform = scaleTransform;
            ellipse.RenderTransformOrigin = new Point(0.5, 0.5); // Center of the ellipse

            // Random positioning on the canvas
            double left = rand.NextDouble() * (StickerCanvas.ActualWidth - ellipse.Width);
            double top = rand.NextDouble() * (StickerCanvas.ActualHeight - ellipse.Height);
            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);

            StickerCanvas.Children.Add(ellipse);

            // Animation setup
            Storyboard storyboard = new Storyboard();
            DoubleAnimation scaleInX = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
            DoubleAnimation scaleInY = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
            DoubleAnimation scaleOutX = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1)) { BeginTime = TimeSpan.FromSeconds(4.9) };
            DoubleAnimation scaleOutY = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1)) { BeginTime = TimeSpan.FromSeconds(4.9) };

            storyboard.Children.Add(scaleInX);
            storyboard.Children.Add(scaleInY);
            storyboard.Children.Add(scaleOutX);
            storyboard.Children.Add(scaleOutY);

            Storyboard.SetTarget(scaleInX, ellipse);
            Storyboard.SetTarget(scaleInY, ellipse);
            Storyboard.SetTarget(scaleOutX, ellipse);
            Storyboard.SetTarget(scaleOutY, ellipse);
            Storyboard.SetTargetProperty(scaleInX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(scaleInY, new PropertyPath("RenderTransform.ScaleY"));
            Storyboard.SetTargetProperty(scaleOutX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(scaleOutY, new PropertyPath("RenderTransform.ScaleY"));

            storyboard.Completed += (s, e) => StickerCanvas.Children.Remove(ellipse);
            storyboard.Begin();
            if (sticker.MediaPath != null)
            {
                mediaPlayer.Play();

                // Optionally, handle media playback events
                mediaPlayer.MediaEnded += (s, e) =>
                {
                    // Actions to perform after the media ends, if necessary
                    mediaPlayer.Close(); // Good practice to close when done
                };
            }
        }

        private void LoadCharAnim(Character character)
        {
            animatedImage.Width = Width*2/5;
            animatedImage.Height = Height / 3;

            // Load the bitmap from the character's image path
            BitmapImage bitmap = new BitmapImage(new Uri(character.ImagePath));
            animatedImage.Source = bitmap;
            double aspectRatio = bitmap.PixelWidth / (double)bitmap.PixelHeight;
      
            RectangleGeometry clip = new RectangleGeometry();
            clip.Rect = new Rect(0, animatedImage.Height * (1 - character.eyeLevel) - animatedImage.Height*aspectRatio / 4, animatedImage.Width, animatedImage.Height*aspectRatio / 2);
            animatedImage.Clip = clip;
            ScaleTransform scale = new ScaleTransform(2/aspectRatio, 2/aspectRatio, 0, 0); // No width scaling, triple height scaling

            // Calculate vertical translation to center the clip
            TranslateTransform translate = new TranslateTransform(-animatedImage.Height /(aspectRatio*2), -animatedImage.Height/aspectRatio * 2 * (1 - character.eyeLevel) + animatedImage.Height /(2));
            
             // Combine scale and translate transformations
             TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(scale);
            transformGroup.Children.Add(translate);
            animatedImage.RenderTransform = transformGroup;



            var hexColor = "#111111"; // Example: Orange color
            var color = (Color)ColorConverter.ConvertFromString(hexColor);
            var brush = new SolidColorBrush(color);

            ImageBorder.Stroke = brush;
                ImageBorder.StrokeThickness = animatedImage.Height/20;
            ImageBorder.Fill = brush;
            
            ImageBorder.Points = new PointCollection
            {
            new Point(0, 0), // Start point shifted to the right for slant effect
            new Point(animatedImage.Width*4/3, 0), // Top right point
            new Point(animatedImage.Width, animatedImage.Height), // Bottom right point, aligns with image bottom
            new Point(-animatedImage.Width/3, animatedImage.Height) // Bottom left point
            };
            leftCover.Stroke = brush;
            leftCover.Fill = brush;
            rightCover.Stroke = brush;
            rightCover.Fill = brush;
            leftCover.Points = new PointCollection
            { 
                new Point(0, 0),
                new Point(animatedImage.Width*1/6,0),
                new Point(-animatedImage.Width/6, animatedImage.Height)
            };
            rightCover.Points = new PointCollection
            {
                new Point(animatedImage.Width*7/6, 0),
                new Point(animatedImage.Width,animatedImage.Height),
                new Point(animatedImage.Width*5/6, animatedImage.Height)
            };
        }
        private void AnimateImage()
        {
            var canvasWidth = this.Width;
            var canvasHeight = this.Height;

            // Set initial positioning of the animationCanvas
            double initialLeftPosition = -(Width * 2);
            Canvas.SetLeft(animCanvas, initialLeftPosition);
            double centerTopPosition = (canvasHeight - animatedImage.Height) / 2;
            Canvas.SetTop(animCanvas, centerTopPosition);

            // Phase 1 Animation
            DoubleAnimation phase1Animation = new DoubleAnimation
            {
                From = initialLeftPosition,
                To = Width/5, // Midway minus half width of the container
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new PowerEase { Power = 2, EasingMode = EasingMode.EaseIn }
            };

            DoubleAnimation midAnimation = new DoubleAnimation
            {
                From = Width/5,
                To = Width/4,
                Duration = TimeSpan.FromSeconds(1),
                BeginTime = TimeSpan.FromSeconds(0.3),
                
            };
            // Phase 2 Animation
            DoubleAnimation phase2Animation = new DoubleAnimation
            {
                From = Width/4,
                To = Width * 3, // To the end beyond the initial right edge
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new PowerEase { Power = 2, EasingMode = EasingMode.EaseOut },
                BeginTime = TimeSpan.FromSeconds(1.3) // Start after phase 1 ends and a slight pause
            };

            // Storyboard setup
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(phase1Animation);
            storyboard.Children.Add(midAnimation);
            storyboard.Children.Add(phase2Animation);

            Storyboard.SetTarget(phase1Animation, animCanvas);
            Storyboard.SetTarget(phase2Animation, animCanvas);
            Storyboard.SetTarget(midAnimation, animCanvas);

            Storyboard.SetTargetProperty(phase1Animation, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(phase2Animation, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(midAnimation, new PropertyPath(Canvas.LeftProperty));


            storyboard.Begin();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            serverManager.StopServer();
            ConfigManager.SaveConfig(_config);
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonsWidth();
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
            AddEnemyButton.Width = Width / 50;
            AddEnemyButton.Height = Width / 50;
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
                if (_config.Parties.Count == 0) { partyComboBox.ItemsSource = _config.Parties; }
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
            if (_config.Parties.Count == 0 || _config.selectedPartyIndex == -1) { return; }
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
            characterManager.NewSkill(character);
            ConfigManager.SaveConfig(_config);
            PopulateCharacterPanels();
        }

        public void StartAnimation(Character character)
        {
            _config = ConfigManager.LoadConfig();
            LoadCharAnim(character);
            AnimateImage();
        }
        private void chooseItemButton_Click(object sender, RoutedEventArgs e)
        {
            SelectionWindow window = new SelectionWindow(_config.Items, Height, Width);
            if (window.ShowDialog() == true)
            {
                var selectedItem = (Item)window.SelectedItem;
                var button = sender as Button;
                if (button == null) return; // Safeguard, should not happen
                var character = button.Tag as Character;
                if (character == null) return;
                characterManager.AddItem(character, selectedItem);
                ConfigManager.SaveConfig(_config);
                PopulateCharacterPanels();
            }
        }
        private void chooseSkillButton_Click(object sender, RoutedEventArgs e)
        {
            SelectionWindow window = new SelectionWindow(_config.Skills, Height, Width);
            if (window.ShowDialog() == true)
            {
                var selectedItem = (Skill)window.SelectedItem;
                var button = sender as Button;
                if (button == null) return; // Safeguard, should not happen
                var character = button.Tag as Character;
                if (character == null) return;
                characterManager.AddSkill(character, selectedItem);
                ConfigManager.SaveConfig(_config);
                PopulateCharacterPanels();
            }
        }
        private void btnAmbush_Click(object sender, RoutedEventArgs e)
        {
            CombatManager.Ambush();
            UpdateUI();
        }
        private void AddEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            
           
            SelectionWindow window = new SelectionWindow(_config.Enemies, Height, Width);
            if (window.ShowDialog() == true)
            {
                var selectedItem = (Enemy)window.SelectedItem;
                var panel = new EnemyPanelControl(selectedItem);
                EnemyPanels.Children.Add(panel);
            }
            
        }
        private void btnNextTurn_Click(object sender, RoutedEventArgs e)
        {
            CombatManager.NextTurn();
            UpdateUI();
            PopulateCharacterPanels();
        }

        private void btnEndCombat_Click(object sender, RoutedEventArgs e)
        {
            CombatManager.EndCombat();
            UpdateUI();
            PopulateCharacterPanels();
        }

        private void UpdateUI()
        {
            // Update the label to reflect the current turn
            lblTurnStatus.Content = $"Turn: {CombatManager.turn}";
            lblTurnStatus.Background = new SolidColorBrush(Colors.White);
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
        public void PopulateCharacterPanels()
        {
            this.Dispatcher.Invoke(() =>
            {
                _config = ConfigManager.LoadConfig();
            int i = 0;
            CharacterPanels.Children.Clear(); // Clear existing panels 
            if (_config.Parties.Count > 0 && _config.selectedPartyIndex >= 0 && _config.selectedPartyIndex < _config.Parties.Count)
            { 
                foreach (Character character in _config.Parties[_config.selectedPartyIndex].Members)
                {
                    int index = i;
                    i++;
                    var panel = new StackPanel
                    {
                        Background = new SolidColorBrush(Colors.LightGray) { Opacity = 0.9 }, // Example styling
                        Margin = new Thickness(5), // Example styling
                        Width = Width / 7, // Example sizing
                        Height = Height * 29 / 30, // Example sizing
                    };
                        var actionTaken = new CheckBox
                        {
                            Content = "Turn Over",
                            IsChecked = character.actionTaken,
                        };
                        actionTaken.Checked += (sender, e) => { character.actionTaken = true;  ConfigManager.SaveConfig(_config); };
                        actionTaken.Unchecked += (sender, e) => { character.actionTaken = false;  ConfigManager.SaveConfig(_config); };
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
                        FontWeight = FontWeights.Bold,
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
                        FontWeight = FontWeights.Bold,
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
                        FontWeight = FontWeights.Bold,
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
                    panel.Children.Add(actionTaken);
                    // Adding elements to the main panel
                    panel.Children.Add(characterNameText);
                    if (characterImage != null) panel.Children.Add(characterImage);
                    //panel.Children.Add(InfoPanel);
                    panel.Children.Add(HPPanel);
                    panel.Children.Add(MPPanel);
                    panel.Children.Add(skillPanel);
                    
                    foreach (var skill in character.Skills)
                    {

                            // Create the Expander
                            var expander = new Expander
                            {
                                Margin = new Thickness(5, -5, 5, -5),
                                Tag = skill, // Store the item object in the Tag for easy access
                                
                        };
                           
                            // Create a TextBox for the item name in the Expander's Header
                            var nameTextBox = new TextBox
                        {
                            Text = skill.Name,
                            Margin = new Thickness(0),
                            BorderThickness = new Thickness(0), // Optionally remove border for a cleaner look
                            Background = new SolidColorBrush(Colors.Transparent), // Optional: make the TextBox background transparent
                            Width = Width / 12 - 20,
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
                        var headerPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                        };
                        var usesTextBox = new TextBox
                        {
                            Text = $"{skill.RemainingUses} / {skill.MaxUses}",
                            Margin = new Thickness(5, 0, 5, 0),
                            BorderThickness = new Thickness(0),
                            Background = new SolidColorBrush(Colors.Transparent),
                            Width = Width / 15, // Adjust width as needed to accommodate the text
                            HorizontalAlignment = HorizontalAlignment.Left,
                            TextAlignment = TextAlignment.Center // Center the text for better aesthetics
                        };
                        usesTextBox.TextChanged += (sender, e) =>
                        {
                            var parts = usesTextBox.Text.Split('/');
                            if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int newRemainingUses) && int.TryParse(parts[1].Trim(), out int newMaxUses))
                            {
                                skill.RemainingUses = newRemainingUses;
                                skill.MaxUses = newMaxUses;
                                ConfigManager.SaveConfig(_config);
                            }
                        };

                        // Add the usesTextBox to the headerPanel
                        
                        // Add the nameTextBox to the headerPanel
                        headerPanel.Children.Add(nameTextBox);
                        headerPanel.Children.Add(usesTextBox);
                        expander.Header = headerPanel;

                        // Create a TextBox for the item's description inside the Expander's content
                        var descriptionTextBox = new TextBox
                        {
                            Text = skill.Description,
                            IsReadOnly = false, // Or false if you want it to be editable
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Thickness(5),
                            AcceptsReturn = true,
                            Background = new SolidColorBrush(Colors.LightGray), // Optional
                            BorderThickness = new Thickness(0),
                            FontSize = 10// Optionally remove border
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
                        var SPBar = new ProgressBar
                        {
                            Margin = new Thickness(0, 2, 0, 2),
                            Height = Height / 80,
                            Width = Width / 8,
                            Value = skill.Cooldown,
                            Maximum = skill.BaseCooldown,
                            Foreground = new SolidColorBrush(Colors.Gold),
                        };
                        var clickableArea = new Border
                        {
                            Background = new SolidColorBrush(Colors.Transparent), // Make the border transparent
                            Child = SPBar
                        };
                        clickableArea.MouseLeftButtonDown += (sender, e) =>
                        {
                            character.useSkill(skill);  // Call the ExecuteSkill() method when the ProgressBar is clicked
                            ConfigManager.SaveConfig(_config);
                            StartAnimation(character);
                            PopulateCharacterPanels();
                        };
                        var SPPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        var plusSP = new Button
                        {
                            BorderThickness = new Thickness(0),
                            Content = ">"
                        };
                        plusSP.Click += (sender, e) => { skill.UpSP(1); ConfigManager.SaveConfig(_config); PopulateCharacterPanels(); };
                        var minusSP = new Button
                        {
                            BorderThickness = new Thickness(0),
                            Content = "<"
                        };
                        minusSP.Click += (sender, e) => { skill.DownSP(1); ConfigManager.SaveConfig(_config); PopulateCharacterPanels(); };
                        SPPanel.Children.Add(minusSP);
                        
                        SPPanel.Children.Add(clickableArea);
                        SPPanel.Children.Add(plusSP);
                        // Add the Expander to the panel
                        panel.Children.Add(expander);
                        if (skill.BaseCooldown > 0) {
                            panel.Children.Add(SPPanel);
                        }

                    }
                    panel.Children.Add(equippedItemsText);
                    foreach (var item in character.EquippedItems)
                    {
                        // Create the Expander
                        var expander = new Expander
                        {
                            Margin = new Thickness(5, -5, 5, -5),
                            Tag = item, // Store the item object in the Tag for easy access

                        };

                        // Create a TextBox for the item name in the Expander's Header
                        var nameTextBox = new TextBox
                        {
                            Text = item.Name,
                            Margin = new Thickness(0),
                            BorderThickness = new Thickness(0), // Optionally remove border for a cleaner look
                            Background = new SolidColorBrush(Colors.Transparent), // Optional: make the TextBox background transparent
                            Width = Width / 12 - 20,
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
                        var headerPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                        };
                        var usesTextBox = new TextBox
                        {
                            Text = $"{item.Count} / {item.MaxUses}",
                            Margin = new Thickness(5, 0, 5, 0),
                            BorderThickness = new Thickness(0),
                            Background = new SolidColorBrush(Colors.Transparent),
                            Width = Width / 15, // Adjust width as needed to accommodate the text
                            HorizontalAlignment = HorizontalAlignment.Right,
                            TextAlignment = TextAlignment.Center // Center the text for better aesthetics
                        };
                        usesTextBox.TextChanged += (sender, e) =>
                        {
                            var parts = usesTextBox.Text.Split('/');
                            if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int newRemainingUses) && int.TryParse(parts[1].Trim(), out int newMaxUses))
                            {
                                item.Count = newRemainingUses;
                                item.MaxUses = newMaxUses;
                                ConfigManager.SaveConfig(_config);
                            }
                        };

                        // Add the usesTextBox to the headerPanel

                        // Add the nameTextBox to the headerPanel
                        headerPanel.Children.Add(nameTextBox);
                        headerPanel.Children.Add(usesTextBox);
                        expander.Header = headerPanel;
                            // Set the Expander's header to the TextBox

                            // Create a TextBox for the item's description inside the Expander's content
                            var descriptionTextBox = new TextBox
                            {
                                Text = item.Description,
                                IsReadOnly = false, // Or false if you want it to be editable
                                TextWrapping = TextWrapping.Wrap,
                                Margin = new Thickness(5),
                                AcceptsReturn = true,
                                Background = new SolidColorBrush(Colors.LightGray), // Optional
                                BorderThickness = new Thickness(0),
                                FontSize = 16// Optionally remove border
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
                            Margin = new Thickness(5, -5, 5, -5),
                            Tag = item, // Store the item object in the Tag for easy access

                        };
                        // Create a TextBox for the item name in the Expander's Header
                        var nameTextBox = new TextBox
                        {
                            Text = item.Name,
                            Margin = new Thickness(0),
                            BorderThickness = new Thickness(0), // Optionally remove border for a cleaner look
                            Background = new SolidColorBrush(Colors.Transparent), // Optional: make the TextBox background transparent
                            Width = Width / 12 - 20,
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
                        var headerPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                        };
                        var usesTextBox = new TextBox
                        {
                            Text = $"{item.Count} / {item.MaxUses}",
                            Margin = new Thickness(5, 0, 5, 0),
                            BorderThickness = new Thickness(0),
                            Background = new SolidColorBrush(Colors.Transparent),
                            Width = Width / 15, // Adjust width as needed to accommodate the text
                            HorizontalAlignment = HorizontalAlignment.Right,
                            TextAlignment = TextAlignment.Center // Center the text for better aesthetics
                        };
                        usesTextBox.TextChanged += (sender, e) =>
                        {
                            var parts = usesTextBox.Text.Split('/');
                            if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int newRemainingUses) && int.TryParse(parts[1].Trim(), out int newMaxUses))
                            {
                                item.Count = newRemainingUses;
                                item.MaxUses = newMaxUses;
                                ConfigManager.SaveConfig(_config);
                            }
                        };

                        // Add the usesTextBox to the headerPanel

                        // Add the nameTextBox to the headerPanel
                        headerPanel.Children.Add(nameTextBox);
                        headerPanel.Children.Add(usesTextBox);
                        expander.Header = headerPanel;

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
            });
        }
    }    
}