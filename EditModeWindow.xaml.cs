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
    /// Interaction logic for EditModeWindow.xaml
    /// </summary>
    public partial class EditModeWindow : Window
    {
        private AppConfig updated_config;

        public EditModeWindow(AppConfig config, double height, double width)
        {
            InitializeComponent();

            updated_config = config;
            // Or however you instantiate it

            // Copy values from old_config to new_config if needed

            this.DataContext = updated_config; // Set DataContext for data binding

            this.Height = height * 0.8;
            this.Width = width * 0.8;
        }

        public void AddCharacterButton_Click(object sender, RoutedEventArgs e)
        {

        }
        public void CreateCharacterButton_Click(object sender, RoutedEventArgs e)
        {

        }
        public void DeleteCharacterButton_Click(object sender, RoutedEventArgs e) { }

        public void CreatePartyButton_Click(object sender, RoutedEventArgs e)
        {

        }
        public void DeletePartyButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
