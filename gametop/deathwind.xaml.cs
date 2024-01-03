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

namespace gametop
{
    /// <summary>
    /// Логика взаимодействия для deathwind.xaml
    /// </summary>

    public interface IGameWindow
    {
        void RestartGame();
    }

    public partial class deathwind : Window
    {
        private IGameWindow game;

        public deathwind(IGameWindow game)
        {
            InitializeComponent();
            this.game = game;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            game.RestartGame();
            this.Close();
        }
    }
}
