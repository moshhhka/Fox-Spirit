﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
using System.Windows.Media.Media3D;
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
        public static int coins { get; set; }
        public static bool gotFood = Boss1.gotFood;

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

            if (door1.Visibility == Visibility.Visible && Canvas.GetLeft(player) < Canvas.GetLeft(door1) + door1.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(door1) &&
                Canvas.GetTop(player) < Canvas.GetTop(door1) + door1.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(door1))
            {
                Player.goLeft = false;
                Player.goRight = false;
                Player.goUp = false;
                Player.goDown = false;
                Kyhnya1 newRoom = new Kyhnya1();
                Kyhnya1.coins = coins;
                this.Hide();
                timer.Stop();
                newRoom.Show();
                
            }

            List<UIElement> elementsCopy = myCanvas.Children.Cast<UIElement>().ToList();

            foreach (UIElement x in elementsCopy)
            {
                if (x is Image image && (string)image.Tag == "map1.png")
                { // Проверяем, что Tag начинается с "map"

                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image), Canvas.GetTop(image), image.ActualWidth, image.ActualHeight);

                    if (rect1.IntersectsWith(rect2) && x.Visibility == Visibility.Visible)
                    {
                        myCanvas.Children.Remove(x);
                        healthBar.Maximum = 150;
                        MessageBox.Show("Вы получили карту \"Быстрее ветра\", которая даёт вам прибавку к скорости +10");
                        Player.goLeft = false;
                        Player.goRight = false;
                        Player.goUp = false;
                        Player.goDown = false;
                    }
                }

                if (x is Image image1 && (string)image1.Tag == "map2.png")
                { // Проверяем, что Tag начинается с "map"

                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image1), Canvas.GetTop(image1), image1.ActualWidth, image1.ActualHeight);

                    if (rect1.IntersectsWith(rect2) && x.Visibility == Visibility.Visible)
                    {
                        myCanvas.Children.Remove(x);
                        Player.speed = 30;
                        MessageBox.Show("Вы получили карту \"Быстрее ветра\", которая даёт вам прибавку к скорости +10");
                        Player.goLeft = false;
                        Player.goRight = false;
                        Player.goUp = false;
                        Player.goDown = false;
                    }
                }

                if (x is Image image2 && (string)image2.Tag == "map3.png")
                { // Проверяем, что Tag начинается с "map"

                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image2), Canvas.GetTop(image2), image2.ActualWidth, image2.ActualHeight);

                    if (rect1.IntersectsWith(rect2) && x.Visibility == Visibility.Visible)
                    {
                        myCanvas.Children.Remove(x);
                        Player.playerHealth -= 10;
                        MessageBox.Show("Вы получили карту \"Быстрее ветра\", которая даёт вам прибавку к скорости +10");
                        Player.goLeft = false;
                        Player.goRight = false;
                        Player.goUp = false;
                        Player.goDown = false;
                    }
                }

            }
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
                Player.goLeft = false;
                Player.goRight = false;
                Player.goUp = false;
                Player.goDown = false;

                if (cardDrawn)
                {
                    MessageBox.Show("Вы уже вытянули карту");
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Хочешь вытянуть карту? Всего 40 монет. Если скажешь нет, я готова поторговаться", "Гадалка:", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (coins > 40)
                    {
                        MessageBox.Show("Вот твоя карта!");
                        Random random = new Random();
                        int randomNumber = random.Next(1, 4);
                        string mapname = "map" + Convert.ToString(randomNumber) + ".png";
                        Image map = new Image();
                        map.Source = new BitmapImage(new Uri(mapname, UriKind.RelativeOrAbsolute));
                        Canvas.SetLeft(map, Canvas.GetLeft(nps1) + 130);
                        Canvas.SetTop(map, Canvas.GetTop(nps1) + 80);
                        map.Tag = mapname;
                        map.Height = 109;
                        map.Width = 105;
                        myCanvas.Children.Add(map);
                        Canvas.SetZIndex(player, 1);
                        cardDrawn = true;
                        coins -= 40;
                        txtCoins.Content = "Coins:" + coins;
                    }

                    else
                    {
                        MessageBox.Show("Как жаль, но у тебя не хватает монет");
                        return;
                    }
                }

                if (result == MessageBoxResult.No)
                {
                    MessageBoxResult foodResult = MessageBox.Show("Так уж и быть, можешь угостить меня чем-то вкусным", "Гадалка:", MessageBoxButton.YesNo);
                    if (foodResult == MessageBoxResult.No)
                    {
                        MessageBox.Show("Прощай");
                        return;
                    }

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
                        map.Tag = mapname;
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
            }

            if (e.Key == Key.E && (Canvas.GetLeft(player) < Canvas.GetLeft(nps) + nps.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(nps) &&
                Canvas.GetTop(player) < Canvas.GetTop(nps) + nps.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(nps)))
            {
                Player.goLeft = false;
                Player.goRight = false;
                Player.goUp = false;
                Player.goDown = false;

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

