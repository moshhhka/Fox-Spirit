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

        int cookieCount = 0;

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
                    Bani1.crist = crist;
                    this.Hide();
                    timer.Stop();
                    newRoom.Show();
                    isKyhnya1Opened = true;
                }
                else if (!isBani1Opened)
                {
                    Bani1 newRoom = new Bani1();
                    Bani1.coins = coins;
                    Bani1.crist = crist;
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

                        cookieCount++;
                        if (cookieCount == 3)
                        {
                            int vospoln = Convert.ToInt16(Player.playerhealthBar.Maximum);
                            Player.playerHealth += vospoln - Player.playerHealth;
                        }
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

                MessageBoxResult result = MessageBox.Show("Здравствуй, путник. Я Гадалка, и я умею читать карты Таро. Это не такое уж и большое искусство, но иногда оно может быть полезно. Я могу показать тебе твою судьбу и дать тебе силу, которая поможет тебе на следующем этаже. Но за это я прошу скромную плату в 30 монет за одну карту. Ты готов заплатить эту цену?", "Гадалка:", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (coins >= 30)
                    {
                        MessageBox.Show("Хм, интересный выбор. Надеюсь, это тебе пригодится. Но не думай, что карта может повлиять на твою судьбу. Ты сам выбираешь свой путь. Карты лишь показывают твои возможности, а не определяют твои действия. Удачи тебе, путник.");
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
                        coins -= 30;
                    }

                    else
                    {
                        MessageBox.Show("Как жаль, но у тебя не хватает монет");
                        return;
                    }
                }

                if (result == MessageBoxResult.No)
                {
                    MessageBoxResult foodResult = MessageBox.Show("Как пожелаешь. Ты упускаешь возможность узнать свое будущее и получить преимущество перед своими врагами. Но может быть, ты хочешь предложить мне что-то другое в обмен? У тебя есть еда? Я очень голодна, и мне нужно подкрепиться, чтобы продолжать гадать. Я делаю это от скуки, но это не значит, что я не нуждаюсь в еде. Если ты поделишься со мной своей пищей, я тоже поделюсь с тобой своим знанием. Что ты на это скажешь?", "Гадалка:", MessageBoxButton.YesNo);
                    if (foodResult == MessageBoxResult.No)
                    {
                        MessageBox.Show("Прощай");
                        return;
                    }

                    if (foodResult == MessageBoxResult.Yes && gotFood == true)
                    {
                        MessageBox.Show("Хм, интересный выбор. Надеюсь, это тебе пригодится. Но не думай, что карта может повлиять на твою судьбу. Ты сам выбираешь свой путь. Карты лишь показывают твои возможности, а не определяют твои действия. Удачи тебе, путник.");
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
                    MessageBox.Show("Привет, путник. Я рад тебя видеть. Я знаю, что ты принес мне новый трофей. Это очень важный предмет, который имеет глубокую историю. Позволь рассказать тебе, откуда он появился…", "Сказитель:");
                    MessageBox.Show("Мэдока был не просто работником бань \"Волшебный Водоем\", а их душой и сердцем. Он заботился о том, чтобы все котельные работали без сбоев, а вода была всегда чистой и горячей. Он также умел находить баланс между трудом и отдыхом, и учил этому своих коллег. Он проводил чайные церемонии, которые были для него священными. Он любил свой чайник, который был подарком от старого мастера, и хранил в нем свои самые лучшие сорта чая. Но однажды все изменилось.", "Сказитель:");
                    MessageBox.Show("Во время одной из церемоний в бани ворвался злобный демон, который хотел уничтожить все, что было дорого Мэдоке. Он схватил чайник и разбил его о пол, а затем напал на Мэдоку и его друзей. Мэдока вступил в бой с демоном, и смог одолеть его благодаря своей силе и мудрости. Мэдока отобрал у демона его череп, который был источником его злой магии. Но череп демона был проклят, и он отравил Мэдоку своим злом. Он постепенно превратил Мэдоку в лиса-демона, который потерял свою доброту и ум. Он забыл о своей прежней жизни и своих друзьях.", "Сказитель:");
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
                    MessageBox.Show("Ты вернул амулет его законному хозяину, путник. Возьми это печенье – оно увеличит твой запас здоровья и поможет тебе в следующих битвах.", "Сказитель:");
                    isTrof1 = false;
                    return;
                }
                if (isTrof2)
                {
                    MessageBox.Show("Привет, путник. Я рад тебя видеть. Я знаю, что ты принес мне новый трофей. Это очень важный предмет, который имеет глубокую историю. Позволь рассказать тебе, откуда он появился…");
                    MessageBox.Show("Сэчико была радостью и светом бань \"Волшебный Водоем\". Она готовила самые вкусные и изысканные блюда, которые поражали воображение гостей. Она также была дружелюбной и общительной, и всегда готова была поделиться своими секретами кулинарии с другими работниками. Но Сэчико имела одну слабость - она любила плюшки-акито-ину, которые ей научила готовить её мама. Это были простые, но очень ароматные и мягкие пирожки с начинкой из красного боба. Сэчико ела их только в особых случаях, когда она чувствовала себя уставшей или грустной. Она хранила их в своем тайном ящичке на кухне, и никому не говорила о них.", "Сказитель:");
                    MessageBox.Show("Но однажды, когда она открыла ящик, она обнаружила, что все плюшки исчезли. Вместо них там лежало письмо, в котором было написано: \"Если хочешь вернуть свои плюшки, приходи в полночь к пруду за банями. И не говори никому, иначе ты пожалеешь\".", "Сказитель:");
                    MessageBox.Show("Сэчико была в шоке и страхе, но она не могла отказаться от своих любимых плюшек. Она решила пойти к пруду, не подозревая, что там её ждет ловушка. Когда она пришла к пруду, она увидела, что там стоял злой демон, который держал в руках её плюшки. Он сказал ей, что он хочет сделать из неё свою невесту, и что если она согласится, он вернет ей плюшки. Сэчико отказалась, но демон не слушал её. Он схватил её и поцеловал её в губы. В тот момент Сэчико почувствовала, как её тело и душа меняются. Она превратилась в девушку-демона с ярко-красными волосами и синими пламенами вокруг себя. Она потеряла свою радость и свет, и стала холодной и жестокой.", "Сказитель:");
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
                    MessageBox.Show("Ты вернул амулет его законному хозяину, путник. Возьми это печенье – оно увеличит твой запас здоровья и поможет тебе в следующих битвах.", "Сказитель:");
                    isTrof2 = false;
                    return;
                }
                if (isTrof3)
                {
                    MessageBox.Show("Привет, путник. Я рад тебя видеть. Я знаю, что ты принес мне новый трофей. Это очень важный предмет, который имеет глубокую историю. Позволь рассказать тебе, откуда он появился…", "Сказитель:");
                    MessageBox.Show("Сэкера была известна во всей Японии как лучшая мастерица настоев. Её зелья могли исцелить от любой болезни, даже от бессонницы, которая считалась неизлечимой. Она работала в банях \"Волшебный Водоем\", где её услугами пользовались самые знатные и богатые люди. Она всегда была доброй и щедрой, и дарила свои настои тем, кто не мог их себе позволить. Но однажды её жизнь изменилась.", "Сказитель:");
                    MessageBox.Show("К ней пришел таинственный незнакомец, который попросил её дать ему зелье от бессонницы. Сэкера не подозревала, что он был злым колдуном, который хотел украсть её секрет. Он подменил её настои, которые она хранила в нефритовом ящичке, ядовитым порошком. Когда Сэкера открыла ящик, то вдохнула яд и превратилась в демона. Её кожа потеряла цвет, а глаза стали светиться зловещим огнем. Она больше не могла создавать настои, а только разрушать и убивать.", "Сказитель:");
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
                    MessageBox.Show("Ты вернул амулет его законному хозяину, путник. Возьми это печенье – оно увеличит твой запас здоровья и поможет тебе в следующих битвах.", "Сказитель:");
                    isTrof3 = false;
                    return;
                }

                if (!isTrof1 && !isTrof2 && !isTrof3)
                {
                    MessageBox.Show("Увы у тебя нет трофеев для меня", "Сказитель:");
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

                MessageBoxResult result = MessageBox.Show("Приветик, странник. Я Оружейница и ты вообще видел мой меч?! Он уже столько демонов перебил… Но не суть! Ты ищешь что-то особенное для своего оружия? Тогда ты пришел по адресу. У меня есть сферы, которые могут сделать твое оружие сильнее, быстрее, красивее… Ну, ты понял. Но не все так просто. Я не раздаю свои сферы просто так. Ты должен заплатить мне 30 кристаллов за каждую. Ну что, соглашаешься? Или ты боишься рисковать?", "Оружейница:", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (crist >= 30)
                    {
                        MessageBox.Show("Ну вот, теперь ты обладатель сферы усиления. Ты не знаешь, что она делает с твоим оружием, пока не попробуешь. Может быть, она сделает его острее, или тяжелее, или светящимся. Или может быть, она взорвется тебе в руках. Шучу, шучу. Или нет? Ладно, не буду тебя дразнить. Вот твоя сфера, и еще одна подсказка: не доверяй всему, что видишь на этом этаже, тут полно ловушек и иллюзий.");
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
                        crist -= 30;
                        txtCoins.Content = crist;
                    }

                    else
                    {
                        MessageBox.Show("Как жаль, но у тебя не хватает кристаллов");
                        return;
                    }
                }

                if (result == MessageBoxResult.No)
                {
                    MessageBoxResult foodResult = MessageBox.Show("Ох, ты такой скучный. Ты не хочешь рискнуть и получить сферу усиления? Ну ладно, твое дело. Но может быть, ты хоть угостишь меня чем-нибудь? Я так давно не ела ничего вкусного. У тебя есть что-нибудь сладкое или соленое?", "Оружейница:", MessageBoxButton.YesNo);
                    if (foodResult == MessageBoxResult.No)
                    {
                        MessageBox.Show("Прощай");
                        return;
                    }

                    if (foodResult == MessageBoxResult.Yes && gotFood == true)
                    {
                        MessageBox.Show("Ну вот, теперь ты обладатель сферы усиления. Ты не знаешь, что она делает с твоим оружием, пока не попробуешь. Может быть, она сделает его острее, или тяжелее, или светящимся. Или может быть, она взорвется тебе в руках. Шучу, шучу. Или нет? Ладно, не буду тебя дразнить. Вот твоя сфера, и еще одна подсказка: не доверяй всему, что видишь на этом этаже, тут полно ловушек и иллюзий.");
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

