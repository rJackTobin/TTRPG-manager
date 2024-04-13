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
using System.Collections.ObjectModel;

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
        private void AddPath_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog(); // Use Windows Forms folder browser
            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string folderPath = folderDialog.SelectedPath;
                if (!updated_config.LibraryPaths.Contains(folderPath))
                {
                    updated_config.LibraryPaths.Add(folderPath);
                }
            }
        }

        private void DeletePath_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryPathList.SelectedItem != null)
            {
                updated_config.LibraryPaths.Remove(LibraryPathList.SelectedItem as string);
            }
        }
        private void AddSticker_Click(object sender, RoutedEventArgs e)
        {
            // Create a new Sticker object
            Sticker newSticker = new Sticker();

            // Open file dialog to select an image
            OpenFileDialog imageFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif",
                Title = "Select an Image for Sticker"
            };

            if (imageFileDialog.ShowDialog() == true)
            {
                newSticker.ImagePath = imageFileDialog.FileName;
                newSticker.Name = System.IO.Path.GetFileNameWithoutExtension(imageFileDialog.FileName);

                // Optionally, open file dialog to select an audio file
                OpenFileDialog audioFileDialog = new OpenFileDialog
                {
                    Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav",
                    Title = "Select an Audio File for Sticker (Optional)",
                    CheckFileExists = false
                };

                if (audioFileDialog.ShowDialog() == true)
                {
                    newSticker.MediaPath = audioFileDialog.FileName;
                }

                // Add the new sticker to the collection if an image was selected
                updated_config.Stickers.Add(newSticker);
            }
            else
            {
                // Inform user that selecting an image is necessary
                MessageBox.Show("You must select an image to create a sticker.", "Image Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void DeleteSticker_Click(object sender, RoutedEventArgs e)
        {
            // Check if any sticker is selected
            if (StickerList.SelectedItem != null)
            {
                Sticker selectedSticker = StickerList.SelectedItem as Sticker;
                if (selectedSticker != null)
                {
                        updated_config.Stickers.Remove(selectedSticker);
                }
            }
            
        }
    }
}
