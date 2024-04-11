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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TTRPG_manager
{
    /// <summary>
    /// Interaction logic for EnemyPanelControl.xaml
    /// </summary>
    
    public partial class EnemyPanelControl : UserControl
    {
        public Enemy Enemy { get; set; }

        public EnemyPanelControl(Enemy enemy)
        {
            InitializeComponent();
            AppConfig _config = ConfigManager.LoadConfig();
            this.Enemy = (Enemy)enemy.Clone();
            this.DataContext = Enemy;
            var parts = _config.Resolution.Split('x');
            this.Width = (int.Parse(parts[0]))/12;
            this.Height = (int.Parse(parts[1]))/5;
            enemyImage.MaxHeight = this.Height/2;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as StackPanel)?.Children.Remove(this);
        }

        private void IncreaseHP_Click(object sender, RoutedEventArgs e)
        {
            if (Enemy.CurrentHP < Enemy.MaxHP) Enemy.CurrentHP++;
        }

        private void DecreaseHP_Click(object sender, RoutedEventArgs e)
        {
            if (Enemy.CurrentHP > 0) Enemy.CurrentHP--;
        }
    }
    
}
