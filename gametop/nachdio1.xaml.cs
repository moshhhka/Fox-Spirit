using System;
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
        public static bool YzeIgral;

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
                YzeIgral = false;
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
                if (!YzeIgral)
                {
                    keyF.Visibility = Visibility.Hidden;
                    dioL.Visibility = Visibility.Hidden;
                    MessageBox.Show("Здравствуйте, Мелисса! Я очень рад, что вы приехали. Мы сильно нуждаемся в вашей помощи. Наша деревушка постоянно подвергается нападениям злых демонов, которые захватили бани “Волшебный Водоем”. Они убивают наших животных, разрушают наши дома, похищают наших детей.", "Безликий:");
                    MessageBox.Show("Мы не можем противостоять им, они слишком сильны и хитры. Они маскируются под сотрудников бань, чтобы заманить нас в ловушку. Мы уже потеряли многих своих близких и друзей. Мы не знаем, что делать. Вы наша единственная надежда. Пожалуйста, помогите нам избавиться от этих чудовищ.", "Безликий:");
                    MessageBox.Show("Вы славитесь своим мастерством владения мечом (ИСПОЛЬЗОВАТЬ Space) и своей способностью призывать лисьи огни (ИСПОЛЬЗОВАТЬ E). Вы можете победить этих демонов и вернуть нам мир и спокойствие. Мы молимся за вас.", "Безликий:");
                    MessageBox.Show("Безликий дает вам взрывные бомбочки (ИСПОЛЬЗОВАТЬ Q), которые будут наносить урон по площади. Он говорит, что это специальные бомбочки, которые он сам изготовил из трав и пороха. Они могут взорваться с большой силой и ослепить демонов своим ярким светом.");
                    MessageBox.Show("Безликий создает дверь для безопасного прохода в бани. Он говорит, что это одна из его магических способностей, которой он пользуется только в чрезвычайных ситуациях. Он объясняет, что дверь ведет в тайный туннель, который соединяет деревушку и бани. Он уверяет вас, что демоны не знают о существовании этого туннеля и что вы сможете проникнуть в бани незаметно.");
                    gotKey = true;
                    door.Visibility = Visibility.Visible;
                }
                else
                {
                    keyF.Visibility = Visibility.Hidden;
                    dioL.Visibility = Visibility.Hidden;
                    MessageBox.Show("Мелисса, ты жива! Я спас тебя от смерти, используя свою магию. Мы в твоём сознании, Мелисса. Я связал свою душу с твоей душой, когда ты вошла в дверь. Я могу воскрешать тебя, пока я сам жив.", "Мужчина в маске:");
                    MessageBox.Show("Это очень опасный и сложный ритуал, но я рискнул ради тебя. Потому что ты единственная, кто может спасти нас от демонов, Мелисса. Ты очень сильная и смелая. Я хочу помочь тебе в твоём бою.", "Мужчина в маске:");
                    MessageBox.Show("Но я не могу перенести тебя в тот момент, где ты была. Ты всегда начинаешь заново, как будто ничего не произошло. Это единственный способ, как я могу тебя воскрешать. Но будь осторожна, Мелисса. Я не знаю, сколько раз я смогу тебя воскрешать. Ты должна использовать свои способности и бомбочки с умом. Я буду следить за тобой и помогать тебе, когда смогу. Но ты должна решать сама, как действовать. Удачи тебе, Мелисса.", "Мужчина в маске:");
                    gotKey = true;
                    door.Visibility = Visibility.Visible;
                }
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
