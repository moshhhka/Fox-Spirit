using System;
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
        private bool cardDrawn, bafDrawn;
        public static int coins, crist;
        public static bool gotFood;
        public static bool isKyhnya1Opened, isBani1Opened, isFinishOpened;
        public static bool isTrof1, isTrof2, isTrof3, isBuffActive;

        DispatcherTimer timer = new DispatcherTimer();

        Dictionary<string, (string, int)> cookies = new Dictionary<string, (string, int)>
        {
            { "cookie1", ("Вы получили \"Рогалик\", который увеличивает ваш максимальный запас здоровья на 30 единиц", 30) },
            { "cookie2", ("Вы получили \"Плюшка\", который увеличивает ваш максимальный запас здоровья на 50 единиц", 50) },
            { "cookie3", ("Вы получили \"Шоколадное печенье\", который увеличивает ваш максимальный запас здоровья на 70 единиц", 70) }
        };

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        public Window1()
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
            player1.Movement();

            txtCoins.Content = coins;
            txtCrist.Content = crist;

            if (Canvas.GetLeft(player) < Canvas.GetLeft(door1) + door1.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(door1) &&
                Canvas.GetTop(player) < Canvas.GetTop(door1) + door1.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(door1))
            {
                Player.goLeft = false;
                Player.goRight = false;
                Player.goUp = false;
                Player.goDown = false;
                if (!isKyhnya1Opened)
                {
                    Kyhnya1 newRoom = new Kyhnya1();
                    Kyhnya1.coins = coins;
                    this.Hide();
                    timer.Stop();
                    newRoom.Show();
                    isKyhnya1Opened = true;
                }
                else if (!isBani1Opened)
                {
                    Bani1 newRoom = new Bani1();
                    Bani1.coins = coins;
                    this.Hide();
                    timer.Stop();
                    newRoom.Show();
                    isBani1Opened = true;
                }
                else if (!isFinishOpened)
                {
                    final newRoom = new final();
                    this.Hide();
                    timer.Stop();
                    newRoom.Show();
                    isFinishOpened = true;
                }
            }

            List<UIElement> elementsCopy = myCanvas.Children.Cast<UIElement>().ToList();

            Dictionary<string, (string, Action<UIElement>)> maps = new Dictionary<string, (string, Action<UIElement>)>
            {
                { "map1.png", ("Вы получили карту \"Божественное вмешательство\", которое восполняет ваше HP до максимума", (x) =>
                    {
                        int vospoln = Convert.ToInt16(Player.playerhealthBar.Maximum);
                        Player.playerHealth += vospoln - Player.playerHealth;
                    }) },
                { "map2.png", ("Вы получили карту \"Быстрее ветра\", которая даёт вам прибавку к скорости +10", (x) =>
                    {
                        Player.speed = 30;
                    }) },
                { "map3.png", ("Вы получили карту \"Подсумок Бесконечности\", количество ваших боеприпасов увеличивается до 10", (x) =>
                    {
                        isBuffActive = true;
                }) }
            };

            Dictionary<string, (string, Action)> buffs = new Dictionary<string, (string, Action)>
            {
                { "baf1.png", ("Вы получили бафф \"Холодное сердце\", который делает ваши пули ледяными. Теперь вы можете замедлять врагов на 3 секунды", () =>
                {
                    MakeBoss.bullet_ice = true;
                    MakeMobe.bullet_ice = true;
                }) },
                { "baf2.png", ("Вы получили бафф \"Лисьи духи\". Теперь ваша атака по площади превращает врагов в безобидных лисичек, но всего на 3 секунды", () =>
                {
                    MakeBoss.foxyball = true;
                    MakeMobe.foxyball = true;
                }) },
                { "baf3.png", ("Вы получили бафф \"Змеиный укус\", который делает ваш клинок ядовитым. Теперь вы можете отравляете врагов и им на протяжении 3 секунд наносится постоянный урон в размере 25", () =>
                {
                    MakeBoss.poisonsworf = true;
                    MakeMobe.poisonsworf = true;
                }) }
            };

            foreach (UIElement x in elementsCopy)
            {
                if (x is Image image && image.Tag != null && cookies.TryGetValue((string)image.Tag, out var cookie))
                {
                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image), Canvas.GetTop(image), image.ActualWidth, image.ActualHeight);

                    if (rect1.IntersectsWith(rect2) && x.Visibility == Visibility.Visible)
                    {
                        myCanvas.Children.Remove(x);
                        MessageBox.Show(cookie.Item1);
                        Player.playerhealthBar.Maximum += cookie.Item2;
                        Player.goLeft = false;
                        Player.goRight = false;
                        Player.goUp = false;
                        Player.goDown = false;
                    }
                }

                if (x is Image image1 && image1.Tag != null && maps.TryGetValue((string)image1.Tag, out var map))
                {
                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image1), Canvas.GetTop(image1), image1.ActualWidth, image1.ActualHeight);

                    if (rect1.IntersectsWith(rect2) && x.Visibility == Visibility.Visible)
                    {
                        myCanvas.Children.Remove(x);
                        MessageBox.Show(map.Item1);
                        map.Item2.Invoke(x);
                        Player.goLeft = false;
                        Player.goRight = false;
                        Player.goUp = false;
                        Player.goDown = false;
                    }
                }

                if (x is Image image2 && image2.Tag != null && buffs.TryGetValue((string)image2.Tag, out var buff))
                {
                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image2), Canvas.GetTop(image2), image2.ActualWidth, image2.ActualHeight);

                    if (rect1.IntersectsWith(rect2) && x.Visibility == Visibility.Visible)
                    {
                        myCanvas.Children.Remove(x);
                        MessageBox.Show(buff.Item1);
                        buff.Item2.Invoke();
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
                myCanvasPAUSE.Visibility = Visibility.Visible;
                timer.Stop();
                Canvas.SetZIndex(myCanvasPAUSE, 9999);
            }

            player1.KeyDown(sender, e);

            if (e.Key == Key.F && (Canvas.GetLeft(player) < Canvas.GetLeft(nps1) + nps1.ActualWidth &&
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
                        Canvas.SetZIndex(stenka, 1);
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
                        Canvas.SetZIndex(stenka, 1);
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

            if (e.Key == Key.F && (Canvas.GetLeft(player) < Canvas.GetLeft(nps) + nps.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(nps) &&
                Canvas.GetTop(player) < Canvas.GetTop(nps) + nps.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(nps)))
            {
                Player.goLeft = false;
                Player.goRight = false;
                Player.goUp = false;
                Player.goDown = false;

                if (isTrof1)
                {
                    MessageBox.Show("фраза1");
                    MessageBox.Show("Вот тебе награда, за трофей!");
                    Image cookie = new Image();
                    cookie.Tag = "cookie1";
                    cookie.Source = new BitmapImage(new Uri("cookie3.png", UriKind.RelativeOrAbsolute));
                    Canvas.SetLeft(cookie, Canvas.GetLeft(nps) + 240);
                    Canvas.SetTop(cookie, Canvas.GetTop(nps) + 130);
                    cookie.Height = 80;
                    cookie.Width = 80;
                    myCanvas.Children.Add(cookie);
                    Canvas.SetZIndex(player, 1);
                    Canvas.SetZIndex(stenka, 1);
                    isTrof1 = false;
                }
                if (isTrof2)
                {
                    MessageBox.Show("фраза1");
                    MessageBox.Show("Вот тебе награда, за трофей!");
                    Image cookie = new Image();
                    cookie.Tag = "cookie2";
                    cookie.Source = new BitmapImage(new Uri("cookie1.png", UriKind.RelativeOrAbsolute));
                    Canvas.SetLeft(cookie, Canvas.GetLeft(nps) + 240);
                    Canvas.SetTop(cookie, Canvas.GetTop(nps) + 130);
                    cookie.Height = 80;
                    cookie.Width = 80;
                    myCanvas.Children.Add(cookie);
                    Canvas.SetZIndex(player, 1);
                    Canvas.SetZIndex(stenka, 1);
                    isTrof2 = false;
                }
                if (isTrof3)
                {
                    MessageBox.Show("фраза1");
                    MessageBox.Show("Вот тебе награда, за трофей!");
                    Image cookie = new Image();
                    cookie.Tag = "cookie3";
                    cookie.Source = new BitmapImage(new Uri("cookie2.png", UriKind.RelativeOrAbsolute));
                    Canvas.SetLeft(cookie, Canvas.GetLeft(nps) + 240);
                    Canvas.SetTop(cookie, Canvas.GetTop(nps) + 130);
                    cookie.Height = 80;
                    cookie.Width = 80;
                    myCanvas.Children.Add(cookie);
                    Canvas.SetZIndex(player, 1);
                    Canvas.SetZIndex(stenka, 1);
                    isTrof3 = false;
                }

                if (!isTrof1 && !isTrof2 && !isTrof3)
                {
                    MessageBox.Show("Увы у тебя нет трофеев для меня");
                }
            }

            if (e.Key == Key.F && (Canvas.GetLeft(player) < Canvas.GetLeft(nps2) + nps2.ActualWidth &&
                Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(nps2) &&
                Canvas.GetTop(player) < Canvas.GetTop(nps2) + nps2.ActualHeight &&
                Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(nps2)))
            {
                Player.goLeft = false;
                Player.goRight = false;
                Player.goUp = false;
                Player.goDown = false;

                if (bafDrawn)
                {
                    MessageBox.Show("Вы уже получили бафф");
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Хочешь получить особый навык? Всего 40 монет. Если скажешь нет, я готова поторговаться", "Оружейница:", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (coins > 40)
                    {
                        MessageBox.Show("Вот твой бафф!");
                        Random random = new Random();
                        int randomNumber = random.Next(1, 4);
                        string mapname = "baf" + Convert.ToString(randomNumber) + ".png";
                        Image map = new Image();
                        map.Source = new BitmapImage(new Uri(mapname, UriKind.RelativeOrAbsolute));
                        Canvas.SetLeft(map, Canvas.GetLeft(nps2) + 20);
                        Canvas.SetTop(map, Canvas.GetTop(nps2) + 80);
                        map.Tag = mapname;
                        map.Height = 80;
                        map.Width = 80;
                        myCanvas.Children.Add(map);
                        Canvas.SetZIndex(player, 1);
                        Canvas.SetZIndex(stenka, 1);
                        bafDrawn = true;
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
                        MessageBox.Show("Вот твой бафф!");
                        Random random = new Random();
                        int randomNumber = random.Next(1, 4);
                        string mapname = "baf" + Convert.ToString(randomNumber) + ".png";
                        Image map = new Image();
                        map.Source = new BitmapImage(new Uri(mapname, UriKind.RelativeOrAbsolute));
                        Canvas.SetLeft(map, Canvas.GetLeft(nps2) + 20);
                        Canvas.SetTop(map, Canvas.GetTop(nps2) + 80);
                        map.Tag = mapname;
                        map.Height = 80;
                        map.Width = 80;
                        myCanvas.Children.Add(map);
                        Canvas.SetZIndex(player, 1);
                        Canvas.SetZIndex(stenka, 1);
                        bafDrawn = true;
                        gotFood = false;
                    }

                    else
                    {
                        MessageBox.Show("Как жаль, но у тебя нет еды для меня");
                        return;
                    }
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            player1.KeyUp(sender, e);
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

