﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
        bool gotKey, gotE;

        DispatcherTimer timer = new DispatcherTimer();

        public nachdio1()
        {
            InitializeComponent();
            myCanvas.Focus();
            player1 = new Player(player, myCanvas);
            timer.Tick += new EventHandler(GameTimer);
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Start();
        }

        private void GameTimer(object sender, EventArgs e)
        {
            Player.playerhealthBar.Visibility = Visibility.Hidden;

            player1.Movement();

            List<UIElement> elementsCopy = myCanvas.Children.Cast<UIElement>().ToList();

            CollisionDetector collisionDetector = new CollisionDetector(player, elementsCopy);
            collisionDetector.DetectCollisions();

            if (Canvas.GetLeft(player) < Canvas.GetLeft(door) + door.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(door) &&
                Canvas.GetTop(player) < Canvas.GetTop(door) + door.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(door) && gotKey == true)
            {
                MainWindow newRoom = new MainWindow();
                this.Hide();
                timer.Stop();
                newRoom.Show();
                Player.goLeft = false;
                Player.goRight = false;
                Player.goUp = false;
                Player.goDown = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                myCanvasPAUSE.Visibility = Visibility.Visible;
                timer.Stop();
                Canvas.SetZIndex(myCanvasPAUSE, 9999);
            }

            player1.KeyDown(sender, e);

            if (!gotE && Canvas.GetLeft(player) < Canvas.GetLeft(scazitel) + scazitel.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(scazitel) &&
                Canvas.GetTop(player) < Canvas.GetTop(scazitel) + scazitel.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(scazitel))
            {
                keyF.Visibility = Visibility.Visible;
                dioL.Visibility = Visibility.Visible;
                gotE = true;
            }

            if (e.Key == Key.F && (Canvas.GetLeft(player) < Canvas.GetLeft(scazitel) + scazitel.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(scazitel) &&
                Canvas.GetTop(player) < Canvas.GetTop(scazitel) + scazitel.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(scazitel)))
            {
                keyF.Visibility = Visibility.Hidden;
                dioL.Visibility = Visibility.Hidden;
                MessageBox.Show("фраза 1", "Мужчина в маске:");
                MessageBox.Show("фраза 2", "Мужчина в маске:");
                MessageBox.Show("фраза 2", "Мужчина в маске:");
                gotKey = true;
                door.Visibility = Visibility.Visible;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            player1.KeyUp(sender, e);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void exitbut_Click(object sender, RoutedEventArgs e)
        {
            if (cont.Visibility == Visibility.Visible)
            {
                timer.Start();
                myCanvasPAUSE.Visibility = Visibility.Collapsed;
            }
        }

        private void cont_Click(object sender, RoutedEventArgs e)
        {
            if (exitbut.Visibility == Visibility.Visible)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
