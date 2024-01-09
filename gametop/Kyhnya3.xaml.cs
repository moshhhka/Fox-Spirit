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
    /// Логика взаимодействия для Kyhnya3.xaml
    /// </summary>
    public partial class Kyhnya3 : Window
    {
        MakeBoss boss1;
        Player player1;
        bool gameOver;
        int ammo = 5;
        bool isChestOpened;
        public static bool gotFood;
        public static int coins, crist;
        Random randNum = new Random();
        int originalspeed = Player.speed;
        ImageSource originalImage;

        List<Image> boxList = new List<Image>();
        List<Bullet> bullets = new List<Bullet>();

        DispatcherTimer timer = new DispatcherTimer();
        public DispatcherTimer speedBoostTimer;

        string[] tags = { "trof1", "trof2", "trof3" };

        public Kyhnya3()
        {
            InitializeComponent();
            myCanvas.Focus();
            List<UIElement> elementsCopy = myCanvas.Children.Cast<UIElement>().ToList();
            SetRandomMobe();
            boss1 = new MakeBoss(player, elementsCopy, myCanvas, door1, chest, stenka);
            player1 = new Player(player, myCanvas);
            RestartGame();

            timer.Tick += new EventHandler(GameTimer);
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Start();

            speedBoostTimer = new DispatcherTimer();
            speedBoostTimer.Interval = TimeSpan.FromMilliseconds(200);
            speedBoostTimer.Tick += SpeedBoostTimer_Tick;
        }

        public static void SetRandomMobe()
        {
            MakeBoss.BossKyhnya = false;
            MakeBoss.BossKotelnaya = false;
            MakeBoss.BossBani = false;

            int choice;
            do
            {
                choice = MakeBoss.randNum.Next(3);
            }
            while (MakeBoss.chosenNumbers.Contains(choice));

            MakeBoss.chosenNumbers.Add(choice);

            switch (choice)
            {
                case 0:
                    MakeBoss.BossKyhnya = true;
                    break;
                case 1:
                    MakeBoss.BossKotelnaya = true;
                    break;
                case 2:
                    MakeBoss.BossBani = true;
                    break;
            }
        }

        public void BulletTimer_Tick()
        {
            foreach (Bullet bullet in bullets.ToList())
            {
                bullet.BulletMove();
            }
        }

        private void SpeedBoostTimer_Tick(object sender, EventArgs e)
        {
            Player.speed = originalspeed;
            speedBoostTimer.Stop();
            player.Source = originalImage;
        }

        private void GameTimer(object sender, EventArgs e)
        {
            BulletTimer_Tick();

            if (Player.playerHealth < 1)
            {
                gameOver = true;
                player.Source = new BitmapImage(new Uri("charecter\\pldie.png", UriKind.RelativeOrAbsolute));
                player.Height = 180;
                player.Width = 220;
                MakeBoss.disTimer.Stop();
                MakeBoss.disbaniTimer.Stop();
                MakeBoss.shootbaniTimer.Stop();
                MakeBoss.shootTimer.Stop();
                Player.playerHealth = 0;
                timer.Stop();
                nachdio1.YzeIgral = true;

                myCanvas1.Visibility = Visibility.Visible;
                Canvas.SetZIndex(myCanvas1, 9999);
            }

            txtAmmo.Content = ammo;
            txtCoins.Content = coins;
            txtCrist.Content = crist;

            player1.Movement();


            if (door1.Visibility == Visibility.Visible && Canvas.GetLeft(player) < Canvas.GetLeft(door1) + door1.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(door1) &&
                Canvas.GetTop(player) < Canvas.GetTop(door1) + door1.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(door1))
            {
                Window1 newRoom = new Window1();
                Window1.coins = coins;
                Window1.crist = crist;
                Window1.gotFood = gotFood;
                this.Hide();
                timer.Stop();
                newRoom.Show();
                Player.goLeft = false;
                Player.goRight = false;
                Player.goUp = false;
                Player.goDown = false;
                MakeBoss.foxyball = false;
                MakeMobe.foxyball = false;
                MakeBoss.poisonsworf = false;
                MakeMobe.poisonsworf = false;
                MakeBoss.bullet_ice = false;
                MakeMobe.bullet_ice = false;
                Window1.isBuffActive = false;
                Player.speed = 20;
            }

            List<UIElement> elementsCopy = myCanvas.Children.Cast<UIElement>().ToList();
            boss1.elementsCopy = elementsCopy;


            CollisionDetector collisionDetector = new CollisionDetector(player, elementsCopy);
            collisionDetector.DetectCollisions();

            boss1.MoveBoss();

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

                if (u is Image image1 && (string)image1.Tag == "cristall") // Сбор кристаллов
                {
                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image1), Canvas.GetTop(image1), image1.Width, image1.Height);

                    if (rect1.IntersectsWith(rect2) && u.Visibility == Visibility.Visible)
                    {
                        u.Visibility = Visibility.Hidden;
                        crist++;
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

                if (u is Image imag11)
                {
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if ((string)imag11.Tag == tags[i])
                        {
                            Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                            Rect rect2 = new Rect(Canvas.GetLeft(imag11), Canvas.GetTop(imag11), imag11.Width, imag11.Height);

                            if (rect1.IntersectsWith(rect2) && u.Visibility == Visibility.Visible)
                            {
                                u.Visibility = Visibility.Hidden;
                                if (i == 0) Window1.isTrof1 = true;
                                if (i == 1) Window1.isTrof2 = true;
                                if (i == 2) Window1.isTrof3 = true;
                            }
                        }
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

                foreach (UIElement j in elementsCopy)
                {

                    if (j is Image image2 && (string)image2.Tag == "mobebullet" && u is Image image3 && (string)image3.Tag == "player")
                    {
                        if (Canvas.GetLeft(image3) < Canvas.GetLeft(image2) + image2.ActualWidth &&
                        Canvas.GetLeft(image3) + image3.ActualWidth > Canvas.GetLeft(image2) &&
                        Canvas.GetTop(image3) < Canvas.GetTop(image2) + image2.ActualHeight &&
                        Canvas.GetTop(image3) + image3.ActualHeight > Canvas.GetTop(image2))
                        {
                            Player.playerHealth -= 2;
                            myCanvas.Children.Remove(image2);
                            image2.Source = null;
                        }
                    }

                    if (j is Image image4 && (string)image4.Tag == "mobesphere" && u is Image image5 && (string)image5.Tag == "player")
                    {
                        if (Canvas.GetLeft(image5) < Canvas.GetLeft(image4) + image4.ActualWidth &&
                        Canvas.GetLeft(image5) + image5.ActualWidth > Canvas.GetLeft(image4) &&
                        Canvas.GetTop(image5) < Canvas.GetTop(image4) + image4.ActualHeight &&
                        Canvas.GetTop(image5) + image5.ActualHeight > Canvas.GetTop(image4))
                        {
                            Player.playerHealth -= 7;
                            myCanvas.Children.Remove(image4);
                            image4.Source = null;
                        }
                    }

                    if (j is Image image6 && (string)image6.Tag == "box" && u is Image image7 && ((string)image7.Tag == "bullet" || (string)image7.Tag == "sword" || (string)image7.Tag == "sphere"))
                    {
                        if (Canvas.GetLeft(image7) < Canvas.GetLeft(image6) + image6.ActualWidth &&
                        Canvas.GetLeft(image7) + image7.ActualWidth > Canvas.GetLeft(image6) &&
                        Canvas.GetTop(image7) < Canvas.GetTop(image6) + image6.ActualHeight &&
                        Canvas.GetTop(image7) + image7.ActualHeight > Canvas.GetTop(image6))
                        {
                            myCanvas.Children.Remove(image6);
                            image6.Source = null;
                            myCanvas.Children.Remove(image7);
                            image7.Source = null;
                        }
                    }
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)  // Клавиши вкл
        {
            if (gameOver == false)
            {
                player1.KeyDown(sender, e);

                if (!isChestOpened && e.Key == Key.F && chest.Visibility == Visibility.Visible &&
                Canvas.GetLeft(player) < Canvas.GetLeft(chest) + chest.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(chest) &&
                Canvas.GetTop(player) < Canvas.GetTop(chest) + chest.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(chest))
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

            if (e.Key == Key.Escape)
            {
                myCanvasPAUSE.Visibility = Visibility.Visible;
                timer.Stop();
                MakeBoss.disTimer.Stop();
                MakeBoss.disbaniTimer.Stop();
                MakeBoss.shootbaniTimer.Stop();
                MakeBoss.shootTimer.Stop();
                Canvas.SetZIndex(myCanvasPAUSE, 9999);
            }

            if (e.Key == Key.LeftShift)
            {
                originalImage = player.Source;

                Player.speed = 45;

                if (Player.facing == "down")
                {
                    player.Source = new BitmapImage(new Uri("charecter\\downs.png", UriKind.RelativeOrAbsolute));
                }

                else if (Player.facing == "up")
                {
                    player.Source = new BitmapImage(new Uri("charecter\\ups.png", UriKind.RelativeOrAbsolute));
                }

                else if (Player.facing == "left")
                {
                    player.Source = new BitmapImage(new Uri("charecter\\lefts.png", UriKind.RelativeOrAbsolute));
                }

                else if (Player.facing == "right")
                {
                    player.Source = new BitmapImage(new Uri("charecter\\rights.png", UriKind.RelativeOrAbsolute));
                }

                speedBoostTimer.Start();
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
            for (int i = 0; i < 4; i++)
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

            if (e.Key == Key.LeftShift)
            {
                SpeedBoostTimer_Tick(sender, e);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShootBullet(string direstion) // Появление пуль
        {
            Bullet shootBullet = new Bullet(bullets);
            bullets.Add(shootBullet);
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
            ammo.Source = new BitmapImage(new Uri("ammo.png", UriKind.RelativeOrAbsolute));
            ammo.Height = 80;
            ammo.Width = 80;
            Canvas.SetLeft(ammo, randNum.Next(10, Convert.ToInt32(myCanvas.Width - 100)));
            Canvas.SetTop(ammo, randNum.Next(80, Convert.ToInt32(myCanvas.Height - 200)));
            ammo.Tag = "ammo";
            myCanvas.Children.Add(ammo);

            Canvas.SetZIndex(ammo, 1);
            Canvas.SetZIndex(player, 1);
        }

        private void RestartGame() // Перезапуск игры
        {
            player.Source = new BitmapImage(new Uri("charecter\\down.png", UriKind.RelativeOrAbsolute));


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

            boxList.Clear();

            for (int i = 0; i < 3; i++)
            {
                MakeBox();
            }

            Canvas.SetZIndex(stenka, 1);

            Player.goUp = false;
            Player.goLeft = false;
            Player.goDown = false;
            Player.goRight = false;
            gameOver = false;

            boss1.MakeBoss1();
            if (Window1.isBuffActive)
            {
                ammo = 10; // Если бафф активирован, устанавливаем количество боеприпасов на 10
            }
            else
            {
                ammo = 5;
            }

            timer.Start();
        }

        private void playb_Click(object sender, RoutedEventArgs e)
        {
            if (playb.Visibility == Visibility.Visible)
            {
                nachdio1 newRoom = new nachdio1();
                this.Hide();
                timer.Stop();
                newRoom.Show();
            }
        }

        private void exitb_Click(object sender, RoutedEventArgs e)
        {
            if (exitb.Visibility == Visibility.Visible)
            {
                Application.Current.Shutdown();
            }
        }

        private void exitbut_Click(object sender, RoutedEventArgs e)
        {
            if (cont.Visibility == Visibility.Visible)
            {
                timer.Start();
                MakeBoss.disTimer.Start();
                MakeBoss.disbaniTimer.Start();
                MakeBoss.shootbaniTimer.Start();
                MakeBoss.shootTimer.Start();
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
