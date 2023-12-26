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
    /// Логика взаимодействия для Room2.xaml
    /// </summary>
    public partial class Room2 : Window
    {
        MakeMobe zombie1;
        Player player1;
        bool gameOver;
        int ammo = 5;
        int zombieSpeed = 3;
        bool gotKey, isChestOpened;
        public static bool gotFood;
        public static int coins { get; set; }
        Random randNum = new Random();

        List<Image> zombieList = new List<Image>();
        List<Image> boxList = new List<Image>();

        DispatcherTimer timer = new DispatcherTimer();

        public Room2()
        {
            InitializeComponent();
            List<UIElement> elementsCopy = myCanvas.Children.Cast<UIElement>().ToList();
            zombie1 = new MakeMobe(player, elementsCopy, zombieList, myCanvas, key, zombieSpeed);
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
                Window1.coins = coins;
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

                if (u is Image imag1 && (string)imag1.Tag == "food") // Сбор коинов
                {
                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(imag1), Canvas.GetTop(imag1), imag1.Width, imag1.Height);

                    if (rect1.IntersectsWith(rect2) && u.Visibility == Visibility.Visible)
                    {
                        u.Visibility = Visibility.Hidden;
                        gotFood = true;
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

            if (zombie1.score == 15 && !isChestOpened) // Добавить !isChestOpened
            {
                chest.Visibility = Visibility.Visible;
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

            if (!isChestOpened && e.Key == Key.F && (Canvas.GetLeft(player) < Canvas.GetLeft(chest) + chest.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(chest) &&
                Canvas.GetTop(player) < Canvas.GetTop(chest) + chest.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(chest)))
            {
                isChestOpened = true;

                chest.Visibility = Visibility.Hidden; // Сундук

                for (int i = 0; i < 20; i++)
                {
                    CreateCristall();
                }
                for (int i = 0; i < 30; i++)
                {
                    CreateCoin();
                }
                CreateFood();

            }
        }

        private void CreateFood()
        {

            int randomNumber = randNum.Next(1, 4);
            string foodname = "f" + Convert.ToString(randomNumber) + ".png";
            Image food = new Image();
            food.Source = new BitmapImage(new Uri(foodname, UriKind.RelativeOrAbsolute));
            food.Tag = "food";
            food.Height = 110;
            food.Width = 110;

            Canvas.SetLeft(food, Canvas.GetLeft(chest) + randNum.Next(-210, 210));
            Canvas.SetTop(food, Canvas.GetTop(chest) + randNum.Next(-210, 210));

            myCanvas.Children.Add(food);
        }

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

            if (e.Key == Key.E && ammo > 0 && gameOver == false)
            {
                ammo--;
                ShootBullet(Player.facing);


                if (ammo < 1)
                {
                    DropAmmo();
                }
            }

            if (e.Key == Key.Space && gameOver == false)
            {
                ShootSword(Player.facing);
            }

            if (e.Key == Key.Q && ammo > 0 && gameOver == false)
            {
                ammo--;
                ShootSphere();

                if (ammo < 1)
                {
                    DropAmmo();
                }
            }

            if (e.Key == Key.Enter && gameOver == true)
            {
                MainWindow newRoom = new MainWindow();
                this.Hide();
                timer.Stop();
                gotKey = false;
                newRoom.Show();
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

        private void ShootSword(string direstion)
        {
            Sword shootSword = new Sword();
            shootSword.direction = direstion;
            shootSword.swordLeft = (int)Math.Round(Canvas.GetLeft(player) + (player.Width / 2) - 70);
            shootSword.swordTop = (int)Math.Round(Canvas.GetTop(player) + (player.Height / 2) - 70);
            shootSword.MakeSword(myCanvas);
        }

        private void ShootSphere()
        {
            HitSpace shootSphere = new HitSpace();
            shootSphere.MakeSphere(myCanvas, player);
            Canvas.SetZIndex(player, 1);
        }

        private void MakeBox() // Создание коробок
        {
            Image box = new Image();
            box.Tag = "box";
            box.Source = new BitmapImage(new Uri("камень2.png", UriKind.RelativeOrAbsolute));
            box.Height = 109;
            box.Width = 105;

            bool isIntersecting; // Проверка на спавн коробок
            do
            {
                isIntersecting = false;
                Canvas.SetLeft(box, randNum.Next(0, 1595));
                Canvas.SetTop(box, randNum.Next(80, 780));

                Rect newBoxRect = new Rect(Canvas.GetLeft(box), Canvas.GetTop(box), box.Width, box.Height);
                foreach (UIElement uiElement in myCanvas.Children)
                {
                    if (uiElement is Image && ((Image)uiElement).Tag is string tag && (tag == "box" || tag == "chest" || tag == "coin" || tag == "key" || tag == "door" || tag == "ammo" || tag == "player"))
                    {
                        Rect existingElementRect = new Rect(Canvas.GetLeft(uiElement), Canvas.GetTop(uiElement), ((Image)uiElement).Width, ((Image)uiElement).Height);
                        if (newBoxRect.IntersectsWith(existingElementRect))
                        {
                            isIntersecting = true;
                            break;
                        }
                    }
                }
            } while (isIntersecting);

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
            player.Source = new BitmapImage(new Uri("charecter\\down.png", UriKind.RelativeOrAbsolute));

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

            zombie1.score = 0;
            ammo = 5;
            coins = 0;

            timer.Start();
        }
    }
}
