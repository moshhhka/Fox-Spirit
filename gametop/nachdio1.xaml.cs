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
using System.Windows.Threading;

namespace gametop
{
    /// <summary>
    /// Логика взаимодействия для nachdio1.xaml
    /// </summary>
    public partial class nachdio1 : Window
    {
        Player player1;

        DispatcherTimer timer = new DispatcherTimer();

        public nachdio1()
        {
            InitializeComponent();
            player1 = new Player(player, myCanvas, healthBar);
            timer.Tick += new EventHandler(GameTimer);
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Start();
        }

        private void GameTimer(object sender, EventArgs e)
        {
            player1.Movement();


            if (Player.playerHealth > 1)
            {
                healthBar.Value = Player.playerHealth;
            }

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
