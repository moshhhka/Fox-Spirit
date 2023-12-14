using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        bool goLeft, goRight, goUp, goDown, gameOver;
        string facing = "up";
        int playerHealth = 100;
        int speed = 20;

        DispatcherTimer timer = new DispatcherTimer();

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        public Window1()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(GameTimer);
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Start();

        }

        private void GameTimer(object sender, EventArgs e)
        {
            if (goLeft == true && Canvas.GetLeft(player) > 0) // Движения гг
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - speed);
            }

            if (goRight == true && Canvas.GetLeft(player) + player.Width < myCanvas.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + speed);
            }

            if (goUp == true && Canvas.GetTop(player) > 80)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) - speed);
            }

            if (goDown == true && Canvas.GetTop(player) + player.Height < myCanvas.Height)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) + speed);
            }
        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }

            if (e.Key == Key.A)
            {
                goLeft = true;
                facing = "left";
                player.Source = new BitmapImage(new Uri("left.png", UriKind.RelativeOrAbsolute));
            }

            if (e.Key == Key.D)
            {
                goRight = true;
                facing = "right";
                player.Source = new BitmapImage(new Uri("right.png", UriKind.RelativeOrAbsolute));
            }

            if (e.Key == Key.W)
            {
                goUp = true;
                facing = "up";
                player.Source = new BitmapImage(new Uri("up.png", UriKind.RelativeOrAbsolute));
            }

            if (e.Key == Key.S)
            {
                goDown = true;
                facing = "down";
                player.Source = new BitmapImage(new Uri("down.png", UriKind.RelativeOrAbsolute));
            }

            if (e.Key == Key.E && (Canvas.GetLeft(player) < Canvas.GetLeft(nps) + nps.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(nps) &&
                Canvas.GetTop(player) < Canvas.GetTop(nps) + nps.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(nps)))
            {
                DialogWindow dialog = new DialogWindow();
                dialog.ShowDialog();
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                goLeft = false;
            }

            if (e.Key == Key.D)
            {
                goRight = false;
            }

            if (e.Key == Key.W)
            {
                goUp = false;
            }

            if (e.Key == Key.S)
            {
                goDown = false;
            }
        }
    }
}
