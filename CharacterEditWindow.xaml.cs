using Microsoft.Win32;
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

namespace TTRPG_manager
{
    /// <summary>
    /// Interaction logic for CharacterEditWindow.xaml
    /// </summary>
    public partial class CharacterEditWindow : Window
    {
        public CharacterEditWindow(double height, double width, Character character)
        {
            InitializeComponent();

            this.Height = height;
            this.Width = width;
            this.DataContext = character;

        }

        private void OnBrowseButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                
                var character = DataContext as Character;
                if (character != null)
                {
                    character.ImagePath = openFileDialog.FileName;
                    BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                    portrait.Source = bitmap;
                }
            }
        }

        private void SaveCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult= false;
        }
    }
}
