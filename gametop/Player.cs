using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace gametop
{
    internal class Player
    {
        public static bool goLeft, goRight, goUp, goDown;
        public static string facing = "up";
        public int ammo;
        public static int speed = 20;
        public static int playerHealth = 100;
        Image player;
        Canvas myCanvas;
        ProgressBar healthBar;

        public Player(Image player, Canvas myCanvas, ProgressBar healthBar, int ammo = 10)
        {
            this.ammo = ammo;
            this.player = player;
            this.myCanvas = myCanvas;
            this.healthBar = healthBar;
        }

        public void KeyDown(object sender, KeyEventArgs e)  // Клавиши вкл
        {
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

            if (goDown == true && Canvas.GetTop(player) + player.Height < myCanvas.Height)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) + speed);
            }

            if (playerHealth > 1)
            {
                healthBar.Value = playerHealth;
            }
        }
    }
}
