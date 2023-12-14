﻿using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
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
using System.Windows.Threading;
using System.Xml.Linq;
using System.Drawing;
using System.Windows.Media.Animation;

namespace gametop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool goLeft, goRight, goUp, goDown, gameOver;
        string facing = "up";
        public int playerHealth = 100;
        int speed = 20;
        int ammo = 10;
        int zombieSpeed = 3;
        int score, coins;
        bool gotKey;
        Random randNum = new Random();

        List<Image> zombieList = new List<Image>();
        List<Image> boxList = new List<Image>();


        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            RestartGame();
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

            else
            {
                gameOver = true;
                player.Source = new BitmapImage(new Uri("300px-Codex_Death.png", UriKind.RelativeOrAbsolute));
                timer.Stop();
            }

            txtAmmo.Content = "Ammo:" + ammo;
            txtScore.Content = "Kills:" + score;
            txtCoins.Content = "Coins:" + coins;

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

            if (Canvas.GetLeft(player) < Canvas.GetLeft(key) + key.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(key) &&
                Canvas.GetTop(player) < Canvas.GetTop(key) + key.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(key))
            {
                gotKey = true;
                key.Visibility = Visibility.Hidden;
            }

            if (Canvas.GetLeft(player) < Canvas.GetLeft(door) + door.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(door) &&
                Canvas.GetTop(player) < Canvas.GetTop(door) + door.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(door) && gotKey)
            {
                Window1 newRoom = new Window1();
                this.Hide();
                timer.Stop();
                gotKey = false;
                newRoom.Show();
            }

            List<UIElement> elementsCopy = myCanvas.Children.Cast<UIElement>().ToList();

            CollisionDetector collisionDetector = new CollisionDetector(player, elementsCopy);
            collisionDetector.DetectCollisions(goLeft, goRight, goDown, goUp);

            foreach (UIElement u in elementsCopy) 
            {

                if (u is Image image && (string)image.Tag == "coin") // Сбор коинов
                {
                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image), Canvas.GetTop(image), image.Width, image.Height);

                    if (rect1.IntersectsWith(rect2) && u.Visibility == Visibility.Visible)
                    {
                        u.Visibility = Visibility.Hidden;
                        coins++;
                    }
                }

                if (u is Image imagee && (string)imagee.Tag == "ammo") // Трата патронов
                {
                    if (Canvas.GetLeft(player) < Canvas.GetLeft(imagee) + imagee.ActualWidth &&
                        Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(imagee) &&
                        Canvas.GetTop(player) < Canvas.GetTop(imagee) + imagee.ActualHeight &&
                        Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(imagee))
                    {
                        myCanvas.Children.Remove(imagee);
                        imagee.Source = null;
                        ammo += 5;
                    }
                }

                if (u is Image image1 && (string)image1.Tag == "mobe") //Движение мобов
                {
                    if (Canvas.GetLeft(player) < Canvas.GetLeft(image1) + image1.ActualWidth &&
                        Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(image1) &&
                        Canvas.GetTop(player) < Canvas.GetTop(image1) + image1.ActualHeight &&
                        Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(image1))
                    {
                        playerHealth -= 1;
                    }

                    if (Canvas.GetLeft(image1) > Canvas.GetLeft(player))
                    {
                        Canvas.SetLeft(image1, Canvas.GetLeft(image1) - zombieSpeed);
                    }

                    if (Canvas.GetLeft(image1) < Canvas.GetLeft(player))
                    {
                        Canvas.SetLeft(image1, Canvas.GetLeft(image1) + zombieSpeed);
                    }

                    if (Canvas.GetTop(image1) > Canvas.GetTop(player))
                    {
                        Canvas.SetTop(image1, Canvas.GetTop(image1) - zombieSpeed);
                    }

                    if (Canvas.GetTop(image1) < Canvas.GetTop(player))
                    {
                        Canvas.SetTop(image1, Canvas.GetTop(image1) + zombieSpeed);
                    }
                }

                foreach (UIElement j in elementsCopy)
                {
                    if (j is Image image2 && (string)image2.Tag == "bullet" && u is Image image3 && (string)image3.Tag == "mobe") //Убийство мобов
                    {
                        if (Canvas.GetLeft(image3) < Canvas.GetLeft(image2) + image2.ActualWidth &&
                        Canvas.GetLeft(image3) + image3.ActualWidth > Canvas.GetLeft(image2) &&
                        Canvas.GetTop(image3) < Canvas.GetTop(image2) + image2.ActualHeight &&
                        Canvas.GetTop(image3) + image3.ActualHeight > Canvas.GetTop(image2))
                        {
                            score++;

                            myCanvas.Children.Remove(image2);
                            image2.Source = null;
                            myCanvas.Children.Remove(image3);
                            image3.Source = null;
                            zombieList.Remove(image3);
                            MakeZombies();
                        }
                    }
                }
            }

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)  // Клавиши вкл
        {
            if (gameOver == true)
            {
                return;
            }

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

            if(e.Key == Key.D)
            {
                goRight = true;
                facing = "right";
                player.Source = new BitmapImage(new Uri("right.png", UriKind.RelativeOrAbsolute));
            }

            if(e.Key == Key.W)
            {
                goUp = true;
                facing = "up";
                player.Source = new BitmapImage(new Uri("up.png", UriKind.RelativeOrAbsolute));
            }

            if(e.Key == Key.S)
            {
                goDown = true;
                facing = "down";
                player.Source = new BitmapImage(new Uri("down.png", UriKind.RelativeOrAbsolute));
            }
        }



        private void Window_KeyUp(object sender, KeyEventArgs e) // Клавиши выкл
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

            if (e.Key == Key.Space && ammo > 0 && gameOver == false)
            {
                ammo--;
                ShootBullet(facing);


                if (ammo < 1)
                {
                    DropAmmo();
                }
            }

            if (e.Key == Key.Enter && gameOver == true)
            {
                RestartGame();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShootBullet(string direstion) // Появление пуль
        {
            Bullet shootBullet = new Bullet();
            shootBullet.direction = direstion;
            shootBullet.bulletLeft = (int)Math.Round(Canvas.GetLeft(player) + (player.Width / 2));
            shootBullet.bulletTop = (int)Math.Round(Canvas.GetTop(player) + (player.Height / 2));
            shootBullet.MakeBullet(myCanvas);
        }

        private void MakeZombies() // Создание мобов
        {
            Image zombie = new Image();
            zombie.Tag = "mobe";
            zombie.Source = new BitmapImage(new Uri("bos1et.png", UriKind.RelativeOrAbsolute));
            Canvas.SetLeft(zombie, randNum.Next(0, 1595));
            Canvas.SetTop(zombie, randNum.Next(80, 780));
            zombie.Height = 296;
            zombie.Width = 302;
            zombieList.Add(zombie);
            myCanvas.Children.Add(zombie);
            Canvas.SetZIndex(player, 1);

        }

        private void MakeBox() // Создание коробок
        {
            Image box = new Image();
            box.Tag = "box";
            box.Source = new BitmapImage(new Uri("камень2.png", UriKind.RelativeOrAbsolute));
            Canvas.SetLeft(box, randNum.Next(0, 1595));
            Canvas.SetTop(box, randNum.Next(80, 780));
            box.Height = 109;
            box.Width = 105;
            boxList.Add(box);
            myCanvas.Children.Add(box);
            Canvas.SetZIndex(player, 1);
        }

        private void DropAmmo() // Создание боеприпасов
        {
            Image ammo = new Image();
            ammo.Source = new BitmapImage(new Uri("coffee-cup-latte-art-top-view-isolated-on-a-transparent-background-png (1).png", UriKind.RelativeOrAbsolute));
            ammo.Height = 80;
            ammo.Width = 80;
            Canvas.SetLeft(ammo, randNum.Next(10, Convert.ToInt32(myCanvas.Width - ammo.Width)));
            Canvas.SetTop(ammo, randNum.Next(80, Convert.ToInt32(myCanvas.Height - ammo.Height)));
            ammo.Tag = "ammo";
            myCanvas.Children.Add(ammo);

            Canvas.SetZIndex(ammo, 1);
            Canvas.SetZIndex(player, 1);
        }

        private void RestartGame() // Перезапуск игры
        {
            player.Source = new BitmapImage(new Uri("down.png", UriKind.RelativeOrAbsolute));

            foreach (Image i in zombieList)
            {
                myCanvas.Children.Remove(i);
            }

            foreach (Image x in boxList)
            {
                myCanvas.Children.Remove(x);
            }

            foreach (UIElement u in myCanvas.Children)
            {
                if (u is Image image && (string)image.Tag == "coin" && u.Visibility == Visibility.Hidden)
                {
                    u.Visibility = Visibility.Visible;
                }
            }

            if (gotKey == true && key.Visibility == Visibility.Hidden)
            {
                key.Visibility = Visibility.Visible;
                    gotKey = false;
            }

            zombieList.Clear();

            for (int i = 0; i < 3; i++)
            {
                MakeZombies();
            }

            boxList.Clear();

            for (int i = 0; i < 3; i++)
            {
                MakeBox();
            }

            goUp = false;
            goLeft = false;
            goDown = false;
            goRight = false;
            gameOver = false;

            playerHealth = 100;
            score = 0;
            ammo = 10;
            coins = 0;

            timer.Start();
        }
    }
}
