using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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
        public int playerHealth;
        int speed = 20;
        private bool cardDrawn = false;

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
            if (playerHealth > 1)
            {
                healthBar.Value = playerHealth;
            }

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
                player.Source = new BitmapImage(new Uri("left.png", UriKind.RelativeOrAbsolute));
            }

            if (e.Key == Key.D)
            {
                goRight = true;
                player.Source = new BitmapImage(new Uri("right.png", UriKind.RelativeOrAbsolute));
            }

            if (e.Key == Key.W)
            {
                goUp = true;
                player.Source = new BitmapImage(new Uri("up.png", UriKind.RelativeOrAbsolute));
            }

            if (e.Key == Key.S)
            {
                goDown = true;
                player.Source = new BitmapImage(new Uri("down.png", UriKind.RelativeOrAbsolute));
            }

            if (e.Key == Key.E && (Canvas.GetLeft(player) < Canvas.GetLeft(nps1) + nps1.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(nps1) &&
                Canvas.GetTop(player) < Canvas.GetTop(nps1) + nps1.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(nps1)))
            {
                if (cardDrawn)
                {
                    MessageBox.Show("Вы уже вытянули карту");
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Хочешь вытянуть карту?", "Гадалка:", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show("Вот твоя карта!");
                    Random random = new Random();
                    int randomNumber = random.Next(1, 4);
                    string mapname = "map" + Convert.ToString(randomNumber) + ".png";
                    Image map = new Image();
                    map.Source = new BitmapImage(new Uri(mapname, UriKind.RelativeOrAbsolute));
                    Canvas.SetLeft(map, Canvas.GetLeft(nps1) + 130);
                    Canvas.SetTop(map, Canvas.GetTop(nps1) + 80);
                    map.Height = 109;
                    map.Width = 105;
                    myCanvas.Children.Add(map);
                    Canvas.SetZIndex(player, 1);
                    cardDrawn = true;
                }
            }

            if (e.Key == Key.E && (Canvas.GetLeft(player) < Canvas.GetLeft(nps) + nps.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(nps) &&
                Canvas.GetTop(player) < Canvas.GetTop(nps) + nps.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(nps)))
            {
                MessageBoxResult result = MessageBox.Show("Хочешь печенье?", "Повар:", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show("Вот твое печенье!");
                    Image cookie = new Image();
                    cookie.Tag = "box";
                    cookie.Source = new BitmapImage(new Uri("cookie.png", UriKind.RelativeOrAbsolute));
                    Canvas.SetLeft(cookie, Canvas.GetLeft(nps) + 240);
                    Canvas.SetTop(cookie, Canvas.GetTop(nps) + 130);
                    cookie.Height = 139;
                    cookie.Width = 135;
                    myCanvas.Children.Add(cookie);
                    Canvas.SetZIndex(player, 1);
                }
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
