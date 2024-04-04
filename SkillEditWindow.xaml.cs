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
    /// Interaction logic for SkillEditWindow.xaml
    /// </summary>
    public partial class SkillEditWindow : Window
    {
        public SkillEditWindow(double height, double width, Skill skill)
        {
            InitializeComponent();

            this.Height = height;
            this.Width = width;
            this.DataContext = skill;

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
