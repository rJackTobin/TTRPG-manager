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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace TTRPG_manager
{
    /// <summary>
    /// Interaction logic for EnemyEditWindow.xaml
    /// </summary>
    public partial class EnemyEditWindow : Window
    {
        public EnemyEditWindow(double height, double width, Enemy enemy)
        {
            this.Height = height;
            this.Width = width;
            this.DataContext = enemy;
        }
        private void OnBrowseButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {

                var enemy = DataContext as Enemy;
                if (enemy != null)
                {
                    enemy.ImagePath = openFileDialog.FileName;
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
            this.DialogResult = false;
        }
    }
    
}
