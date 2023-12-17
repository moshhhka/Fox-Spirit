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
        Player player1;
        private bool cardDrawn = false;
        public static int coins = MainWindow.coins;
        public static bool gotFood = MainWindow.gotFood;

        DispatcherTimer timer = new DispatcherTimer();

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        public Window1()
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

            txtCoins.Content = "Coins:" + coins;
        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }

            player1.KeyDown(sender, e);
            
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

                MessageBoxResult result = MessageBox.Show("Хочешь вытянуть карту? Всего 50 монет. Если скажешь нет, я готова поторговаться", "Гадалка:", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes && coins > 10)
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
                    coins -= 10;
                    txtCoins.Content = "Coins:" + coins;
                }

                if (result == MessageBoxResult.No)
                {
                    MessageBoxResult foodResult = MessageBox.Show("Так уж и быть, можешь угостить меня чем-то вкусным", "Гадалка:", MessageBoxButton.YesNo);
                    if (foodResult == MessageBoxResult.Yes && gotFood == true)
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
                        gotFood = false;
                    }

                    else
                    {
                        MessageBox.Show("Как жаль, но у тебя нет еды для меня");
                        return;
                    }
                }


                else
                {
                    MessageBox.Show("Как жаль, но у тебя не хватает монет");
                    return;
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
            player1.KeyUp(sender, e);
        }
    }
}
