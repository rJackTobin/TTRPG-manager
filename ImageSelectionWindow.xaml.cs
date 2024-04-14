using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic;
using System.Linq;
using TTRPG_manager;
using System.IO;
using System.Collections.ObjectModel;

namespace Genesis
{
    public class ImageItem
    {
        public string ImageName { get; set; } // Name of the image
        public string ImagePath { get; set; } // Full path to the image
        

        // Constructor
        public ImageItem(string name, string imagePath) { 
            ImageName = name;
            ImagePath = imagePath;
            
        }
    }
    public partial class ImageSelectionWindow : Window
    {
        public delegate void ImageSelectedHandler(string imagePath);
        public event ImageSelectedHandler ImageSelected;
        public string SelectedImagePath { get; private set; }
        private List<ImageItem> _allImageItems;
        public ImageSelectionWindow()
        {
            
            AppConfig _config = ConfigManager.LoadConfig();
            InitializeComponent();
            _allImageItems = new List<ImageItem>();
            LoadImagesFromPaths(_config.LibraryPaths);
            ImageListView.ItemsSource = _allImageItems;
        }
        private void LoadImagesFromPaths(ObservableCollection<string> paths)
        {
            foreach (string path in paths)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                             .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".bmp") || s.EndsWith(".webp"));
                        foreach (var file in files)
                        {
                            var imageName = Path.GetFileName(file);
                            _allImageItems.Add(new ImageItem(imageName, file));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading images: " + ex.Message);
                }
            }
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchTextBox.Text.ToLower();
            var filteredItems = _allImageItems.Where(item => item.ImageName.ToLower().Contains(searchText)).ToList();
            ImageListView.ItemsSource = filteredItems;
        }
        private void ImageListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ImageListView.SelectedItem is ImageItem selectedItem)
            {
                SelectedImagePath = selectedItem.ImagePath;
            }
            else
            {
                SelectedImagePath = null; // Or set a default/fallback path
            }
        }
        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (ImageItem)ImageListView.SelectedItem;
            if (selectedItem != null)
            {
                ImageSelected?.Invoke(selectedItem.ImagePath);
                // Don't close the window
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }

}
