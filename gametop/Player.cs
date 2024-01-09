using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace gametop
{
    internal class Player
    {
        public static bool goLeft, goRight, goUp, goDown;
        public static string facing = "up";
        public static int speed = 20;
        public static int playerHealth = 500;
        public static int speedLeft = 20;
        public static int speedRight = 20;
        public static int speedUp = 20;
        public static int speedDown = 20;
        Image player;
        Canvas myCanvas;
        public static ProgressBar playerhealthBar;


        public Player(Image player, Canvas myCanvas)
        {
            this.player = player;
            this.myCanvas = myCanvas;

            playerhealthBar = new ProgressBar();
            playerhealthBar.Width = 266;
            playerhealthBar.Height = 28;
            playerhealthBar.Value = playerHealth; // Устанавливаем здоровье зомби
            playerhealthBar.Maximum = 500; // Устанавливаем максимальное значение ProgressBar
            // Размещаем ProgressBar над зомби
            Canvas.SetLeft(playerhealthBar, 1636);
            Canvas.SetTop(playerhealthBar, 137);
            myCanvas.Children.Add(playerhealthBar); // Добавляем ProgressBar на Canvas

        }


        public void KeyDown(object sender, KeyEventArgs e)  // Клавиши вкл
        {
            if (e.Key == Key.A)
            {
                goLeft = true;
                facing = "left";
                player.Source = new BitmapImage(new Uri("charecter\\left.png", UriKind.RelativeOrAbsolute));
            }

            if (e.Key == Key.D)
            {
                goRight = true;
                facing = "right";
                player.Source = new BitmapImage(new Uri("charecter\\right.png", UriKind.RelativeOrAbsolute));
            }

            if (e.Key == Key.W)
            {
                goUp = true;
                facing = "up";
                player.Source = new BitmapImage(new Uri("charecter\\up.png", UriKind.RelativeOrAbsolute));
            }

            if (e.Key == Key.S)
            {
                goDown = true;
                facing = "down";
                player.Source = new BitmapImage(new Uri("charecter\\down.png", UriKind.RelativeOrAbsolute));
            }
        }

        public void KeyUp(object sender, KeyEventArgs e) // Клавиши выкл
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

        public void Movement()
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

            if (goDown == true && Canvas.GetTop(player) + player.Height + 120 < myCanvas.Height)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) + speed);
            }

            if (playerHealth > 1)
            {
                playerhealthBar.Value = playerHealth;
            }
        }
    }
}
