using System;
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
        MakeMobe zombie1;
        Player player1;
        bool gameOver;
        int ammo = 10;
        int zombieSpeed = 3;
        int coins;
        bool gotKey;
        Random randNum = new Random();

        List<Image> zombieList = new List<Image>();
        List<Image> boxList = new List<Image>();

        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            List<UIElement> elementsCopy = myCanvas.Children.Cast<UIElement>().ToList();
            zombie1 = new MakeMobe(player, elementsCopy, zombieList, myCanvas, key, chest, zombieSpeed);
            player1 = new Player(player, myCanvas, healthBar);
            RestartGame();
            timer.Tick += new EventHandler(GameTimer);
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Start();
            
        }

        private void GameTimer(object sender, EventArgs e)
        {

            if (Player.playerHealth > 1)
            {
                healthBar.Value = Player.playerHealth;
            }

            else
            {
                gameOver = true;
                player.Source = new BitmapImage(new Uri("300px-Codex_Death.png", UriKind.RelativeOrAbsolute));
                timer.Stop();
            }

            txtAmmo.Content = "Ammo:" + ammo;
            txtScore.Content = "Kills:" + zombie1.score;
            txtCoins.Content = "Coins:" + coins;

            player1.Movement();

            if (key.Visibility == Visibility.Visible && (Canvas.GetLeft(player) < Canvas.GetLeft(key) + key.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(key) &&
                Canvas.GetTop(player) < Canvas.GetTop(key) + key.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(key)))
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
                Player.goLeft = false;
                Player.goRight = false;
                Player.goUp = false;
                Player.goDown = false;
            }

            List<UIElement> elementsCopy = myCanvas.Children.Cast<UIElement>().ToList();
            zombie1.elementsCopy = elementsCopy;


            CollisionDetector collisionDetector = new CollisionDetector(player, elementsCopy);
            collisionDetector.DetectCollisions();

            zombie1.MoveMobe();

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

            player1.KeyDown(sender, e);

            if (e.Key == Key.E)
            {
                chest.Visibility = Visibility.Hidden;

                for (int i = 0; i < 20; i++)
                {
                    CreateCristall();
                }
                for (int i = 0; i < 30; i++)
                {
                    CreateCoin();
                }

            }
        }

        //private void CreateItem(string type)
        //{
        //    // Создать новый элемент
        //    Image item = new Image();
        //    item.Source = new BitmapImage(new Uri("images/" + type + ".png", UriKind.Relative));

        //    // Установить положение элемента на том же месте, где был сундук
        //    Canvas.SetLeft(item, Canvas.GetLeft(chest));
        //    Canvas.SetTop(item, Canvas.GetTop(chest));

        //    // Добавить элемент на холст
        //    myCanvas.Children.Add(item);
        //}

        private void CreateCoin()
        {

            Image coin = new Image();
            coin.Source = new BitmapImage(new Uri("монета.png", UriKind.Relative));
            coin.Tag = "coin";
            coin.Height = 40;
            coin.Width = 40;
            Canvas.SetLeft(coin, Canvas.GetLeft(chest) + randNum.Next(-110, 110));
            Canvas.SetTop(coin, Canvas.GetTop(chest) + randNum.Next(-110, 110));

            myCanvas.Children.Add(coin);
        }

        private void CreateCristall()
        {
            
            Image cristall = new Image();
            cristall.Source = new BitmapImage(new Uri("cristall.png", UriKind.Relative));
            cristall.Tag = "cristall";
            cristall.Height = 50;
            cristall.Width = 40;
            Canvas.SetLeft(cristall, Canvas.GetLeft(chest) + randNum.Next(-110, 110));
            Canvas.SetTop(cristall, Canvas.GetTop(chest) + randNum.Next(-110, 110));

            myCanvas.Children.Add(cristall);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e) // Клавиши выкл
        {
            player1.KeyUp(sender, e);

            if (e.Key == Key.Space && ammo > 0 && gameOver == false)
            {
                ammo--;
                ShootBullet(Player.facing);


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

            for (int i = 0; i < 0; i++)
            {
                zombie1.MakeZombies();
            }

            boxList.Clear();

            for (int i = 0; i < 3; i++)
            {
                MakeBox();
            }

            Player.goUp = false;
            Player.goLeft = false;
            Player.goDown = false;
            Player.goRight = false;
            gameOver = false;

            Player.playerHealth = 100;
            zombie1.score = 0;
            ammo = 10;
            coins = 0;

            timer.Start();
        }
    }
}
