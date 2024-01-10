using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace gametop
{
    internal class MakeBoss
    {

        public string direction;
        Image player;
        Image door1;
        Image chest;
        Image stenka;
        Canvas myCanvas;
        public List<UIElement> elementsCopy;
        public static int bossSpeed = 2;
        ProgressBar bossHealthBar;
        Random rand = new Random();
        bool obezdvijivanie;

        public static bool bullet_ice, poisonsworf, foxyball;

        // Добавьте новый таймер для восстановления скорости зомби после замораживания
        System.Timers.Timer freezeTimer = null;

        public static DispatcherTimer disTimer = new DispatcherTimer();
        public static DispatcherTimer shootTimer = new DispatcherTimer();
        public static DispatcherTimer disbaniTimer = new DispatcherTimer();
        public static DispatcherTimer shootbaniTimer = new DispatcherTimer();

        public static bool BossKyhnya, BossKotelnaya, BossBani;
        public static bool WasBossKyhnya, WasBossKotelnaya, WasBossBani;
        public static List<int> chosenNumbers = new List<int>();
        public static Random randNum = new Random();


        public MakeBoss(Image player, List<UIElement> elementsCopy, Canvas myCanvas, Image door1, Image chest, Image stenka)
        {
            this.player = player;
            this.myCanvas = myCanvas;
            this.elementsCopy = elementsCopy;
            this.door1 = door1;
            this.chest = chest;
            this.stenka = stenka;

            disTimer.Interval = TimeSpan.FromMilliseconds(5000);
            disTimer.Tick += new EventHandler(disTimerEvent);
            disTimer.Start();

            shootTimer.Interval = TimeSpan.FromMilliseconds(2300);
            shootTimer.Tick += new EventHandler(shootTimerEvent);
            shootTimer.Start();

            disbaniTimer.Interval = TimeSpan.FromMilliseconds(5000);
            disbaniTimer.Tick += new EventHandler(disbaniTimerEvent);
            disbaniTimer.Start();

            shootbaniTimer.Interval = TimeSpan.FromMilliseconds(2300);
            shootbaniTimer.Tick += new EventHandler(shootbaniTimerEvent);
            shootbaniTimer.Start();
        }

        public async void disbaniTimerEvent(object sender, EventArgs e)
        {
            if (!BossBani) return;

            foreach (UIElement u in elementsCopy)
            {
                if (u is Image image1 && (string)image1.Tag == "boss")
                {
                    // Сохраняем первоначальное изображение игрока
                    ImageSource playerOriginalImage = player.Source;

                    double playerOriginalLeft = Canvas.GetLeft(player);
                    double playerOriginalTop = Canvas.GetTop(player);

                    // Скрываем босса
                    image1.Visibility = Visibility.Hidden;

                    // Ждем 1 секунду
                    await Task.Delay(500);

                    // Меняем изображение игрока
                    if (Player.facing == "down")
                    {
                        player.Source = new BitmapImage(new Uri("charecter\\downr.png", UriKind.RelativeOrAbsolute));
                    }

                    else if (Player.facing == "up")
                    {
                        player.Source = new BitmapImage(new Uri("charecter\\upr.png", UriKind.RelativeOrAbsolute));
                    }

                    else if (Player.facing == "left")
                    {
                        player.Source = new BitmapImage(new Uri("charecter\\leftr.png", UriKind.RelativeOrAbsolute));
                    }

                    else if (Player.facing == "right")
                    {
                        player.Source = new BitmapImage(new Uri("charecter\\rightr.png", UriKind.RelativeOrAbsolute));
                    }

                    // Ждем еще 1 секунду
                    await Task.Delay(500);

                    // Генерируем случайные координаты для босса, которые находятся на расстоянии не менее minDistance от игрока
                    double bossLeft, bossTop;

                    bossLeft = rand.Next(0, 1595);
                    bossTop = rand.Next(80, 780);
                    image1.Height = 357;
                    image1.Width = 302;


                    // Перемещаем босса на новое место
                    Canvas.SetLeft(image1, bossLeft);
                    Canvas.SetTop(image1, bossTop);


                    for (int i = 0; i < 3; i++)
                    {
                        MobeHitSpace mobeHitSpace = new MobeHitSpace();
                        mobeHitSpace.MakeSphere(myCanvas, player);

                        mobeHitSpace.sphereLeft = (int)(playerOriginalLeft + rand.Next(-300, 320));
                        mobeHitSpace.sphereTop = (int)(playerOriginalTop + rand.Next(-300, 320));

                        Canvas.SetLeft(mobeHitSpace.sphere, mobeHitSpace.sphereLeft);
                        Canvas.SetTop(mobeHitSpace.sphere, mobeHitSpace.sphereTop);
                    }

                    // Показываем босса
                    image1.Visibility = Visibility.Visible;

                    obezdvijivanie = true;

                    await Task.Delay(200);

                    // Возвращаем игроку его первоначальное изображение
                    player.Source = playerOriginalImage;

                    await Task.Delay(1500);

                    image1.Height = 181;
                    image1.Width = 153;
                    obezdvijivanie = false;
                }
            }
        }

        private void shootTimerEvent(object sender, EventArgs e)
        {
            if (!BossKyhnya) return;

            foreach (UIElement u in elementsCopy)
            {
                if (Player.playerHealth <= 0)
                {
                    shootTimer.Stop();
                    return; // Выход из метода
                }

                // Если здоровье игрока больше 0, но таймер остановлен, запустите таймер
                if (Player.playerHealth > 0 && !shootTimer.IsEnabled)
                {
                    shootTimer.Start();
                }

                if (u is Image image1 && (string)image1.Tag == "boss") //Движение мобов
                {
                    // Определите все возможные направления
                    string[] directions = new string[] { "up", "down", "left", "right", "upleft", "upright", "downleft", "downright" };

                    // Создайте пулю в каждом направлении
                    foreach (string direction in directions)
                    {
                        MobeBullet shootBullet = new MobeBullet();
                        shootBullet.direction = direction;
                        shootBullet.bulletLeft = (int)Math.Round(Canvas.GetLeft(image1) + (image1.Width / 2));
                        shootBullet.bulletTop = (int)Math.Round(Canvas.GetTop(image1) + (image1.Height / 2));
                        string bulname = "mobebullet.png";
                        shootBullet.MakeMobeBullet(myCanvas, bulname);
                    }
                }
            }
        }

        private async void shootbaniTimerEvent(object sender, EventArgs e)
        {
            if (!BossBani) return;

            foreach (UIElement u in elementsCopy)
            {
                if (Player.playerHealth <= 0)
                {
                    shootTimer.Stop();
                    return; // Выход из метода
                }

                // Если здоровье игрока больше 0, но таймер остановлен, запустите таймер
                if (Player.playerHealth > 0 && !shootTimer.IsEnabled)
                {
                    shootTimer.Start();
                }

                if (u is Image image1 && (string)image1.Tag == "boss") //Движение мобов
                {
                    string direction = CalculateDirection(Canvas.GetLeft(image1), Canvas.GetTop(image1), Canvas.GetLeft(player), Canvas.GetTop(player));
                    int bulletLeft = (int)Math.Round(Canvas.GetLeft(image1) + (image1.Width / 2));
                    int bulletTop = (int)Math.Round(Canvas.GetTop(image1) + (image1.Height / 2));

                    for (int i = 0; i < 3; i++)
                    {
                        MobeBullet shootBullet = new MobeBullet();
                        shootBullet.direction = direction;
                        shootBullet.bulletLeft = bulletLeft;
                        shootBullet.bulletTop = bulletTop;

                        int randomNumber = rand.Next(1, 6);
                        string bulname = "mobbul" + Convert.ToString(randomNumber) + ".png";

                        shootBullet.MakeMobeBullet(myCanvas, bulname);

                        await Task.Delay(100); // Задержка в 500 миллисекунд
                    }
                }
            }
        }

        public string CalculateDirection(double zombieLeft, double zombieTop, double playerLeft, double playerTop)
        {
            // Вычислите разницу между координатами зомби и игрока
            double deltaX = playerLeft - zombieLeft;
            double deltaY = playerTop - zombieTop;

            // Вычислите угол между зомби и игроком
            double angle = Math.Atan2(deltaY, deltaX);

            // Определите направление от зомби к игроку
            if (angle >= -Math.PI / 8 && angle < Math.PI / 8)
            {
                return "right";
            }
            else if (angle >= Math.PI / 8 && angle < 3 * Math.PI / 8)
            {
                return "downright";
            }
            else if (angle >= 3 * Math.PI / 8 && angle < 5 * Math.PI / 8)
            {
                return "down";
            }
            else if (angle >= 5 * Math.PI / 8 && angle < 7 * Math.PI / 8)
            {
                return "downleft";
            }
            else if (angle >= 7 * Math.PI / 8 || angle < -7 * Math.PI / 8)
            {
                return "left";
            }
            else if (angle >= -7 * Math.PI / 8 && angle < -5 * Math.PI / 8)
            {
                return "upleft";
            }
            else if (angle >= -5 * Math.PI / 8 && angle < -3 * Math.PI / 8)
            {
                return "up";
            }
            else // angle >= -3 * Math.PI / 8 && angle < -Math.PI / 8
            {
                return "upright";
            }
        }

        public void MakeBoss1()
        {
            Image boss = new Image();
            if (BossBani)
            {
                boss.Source = new BitmapImage(new Uri("bosban.png", UriKind.RelativeOrAbsolute));
                MakeBoss.bossSpeed = 8;
                boss.Height = 181;
                boss.Width = 153;
            }
            else if (BossKyhnya)
            {
                boss.Source = new BitmapImage(new Uri("Босс1.png", UriKind.RelativeOrAbsolute));
                MakeBoss.bossSpeed = 2;
                boss.Height = 366;
                boss.Width = 374;
            }
            else if (BossKotelnaya)
            {
                boss.Source = new BitmapImage(new Uri("boss1.png", UriKind.RelativeOrAbsolute));
                MakeBoss.bossSpeed = 2;
                boss.Height = 408;
                boss.Width = 300;
            }
            boss.Tag = "boss";

            Canvas.SetLeft(boss, 810);
            Canvas.SetTop(boss, 336);

            bossHealthBar = new ProgressBar();
            bossHealthBar.Height = 40;
            bossHealthBar.Width = 1500;
            bossHealthBar.Maximum = 2000;
            bossHealthBar.Value = 2000;

            Canvas.SetLeft(bossHealthBar, 207);
            Canvas.SetTop(bossHealthBar, 997);

            myCanvas.Children.Add(boss);

            Canvas.SetZIndex(player, 1);
            Canvas.SetZIndex(stenka, 1);

            myCanvas.Children.Add(bossHealthBar);
            Canvas.SetZIndex(bossHealthBar, 1);
        }

        System.Timers.Timer timer = null;

        public async void disTimerEvent(object sender, EventArgs e)
        {
            if (!BossKotelnaya) return;

            foreach (UIElement u in elementsCopy)
            {
                if (u is Image image1 && (string)image1.Tag == "boss")
                {
                    // Сохраняем первоначальное изображение игрока
                    ImageSource playerOriginalImage = player.Source;

                    // Сохраняем первоначальное положение босса
                    double playerOriginalLeft = Canvas.GetLeft(player);
                    double playerOriginalTop = Canvas.GetTop(player);


                    // Скрываем босса
                    image1.Visibility = Visibility.Hidden;

                    // Ждем 1 секунду
                    await Task.Delay(500);

                    // Меняем изображение игрока
                    if (Player.facing == "down")
                    {
                        player.Source = new BitmapImage(new Uri("charecter\\downr.png", UriKind.RelativeOrAbsolute));
                    }

                    else if (Player.facing == "up")
                    {
                        player.Source = new BitmapImage(new Uri("charecter\\upr.png", UriKind.RelativeOrAbsolute));
                    }

                    else if (Player.facing == "left")
                    {
                        player.Source = new BitmapImage(new Uri("charecter\\leftr.png", UriKind.RelativeOrAbsolute));
                    }

                    else if (Player.facing == "right")
                    {
                        player.Source = new BitmapImage(new Uri("charecter\\rightr.png", UriKind.RelativeOrAbsolute));
                    }

                    // Ждем еще 1 секунду
                    await Task.Delay(500);

                    // Возвращаем босса на его первоначальное место
                    Canvas.SetLeft(image1, playerOriginalLeft);
                    Canvas.SetTop(image1, playerOriginalTop);

                    // Показываем босса
                    image1.Visibility = Visibility.Visible;

                    await Task.Delay(200);

                    // Возвращаем игроку его первоначальное изображение
                    player.Source = playerOriginalImage;
                }
            }
        }

        public void MoveBoss()
        {
            foreach (UIElement u in elementsCopy)
            {
                if (u is Image image1 && (string)image1.Tag == "boss" && (BossKotelnaya || BossKyhnya)) //Движение мобов
                {
                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.RenderSize.Width, player.RenderSize.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image1), Canvas.GetTop(image1), image1.RenderSize.Width, image1.RenderSize.Height);

                    if (rect1.IntersectsWith(rect2))
                    {

                        if (timer == null)
                        {
                            timer = new System.Timers.Timer(500);
                            timer.Elapsed += (sender, e) =>
                            {
                                Player.playerHealth -= 7; // Уменьшите здоровье на 5 через секунду
                                timer.Stop(); // Остановите таймер
                                timer = null; // Установите таймер в null
                            };
                            timer.AutoReset = false; // Установите AutoReset в false, чтобы таймер сработал только один раз
                            timer.Start(); // Запустите таймер
                        }

                    }

                    if (Canvas.GetLeft(image1) > Canvas.GetLeft(player))
                    {
                        Canvas.SetLeft(image1, Canvas.GetLeft(image1) - bossSpeed);
                    }

                    if (Canvas.GetLeft(image1) < Canvas.GetLeft(player))
                    {
                        Canvas.SetLeft(image1, Canvas.GetLeft(image1) + bossSpeed);
                    }

                    if (Canvas.GetTop(image1) > Canvas.GetTop(player))
                    {
                        Canvas.SetTop(image1, Canvas.GetTop(image1) - bossSpeed);
                    }

                    if (Canvas.GetTop(image1) < Canvas.GetTop(player))
                    {
                        Canvas.SetTop(image1, Canvas.GetTop(image1) + bossSpeed);
                    }
                }

                if (u is Image image4 && (string)image4.Tag == "boss" && BossBani) //Движение мобов
                {
                    if (Canvas.GetLeft(image4) > Canvas.GetLeft(player) && Canvas.GetLeft(image4) + image4.Width < myCanvas.Width && obezdvijivanie == false)
                    {
                        Canvas.SetLeft(image4, Canvas.GetLeft(image4) + bossSpeed);
                    }

                    if (Canvas.GetLeft(image4) < Canvas.GetLeft(player) && Canvas.GetLeft(image4) > 0 && obezdvijivanie == false)
                    {
                        Canvas.SetLeft(image4, Canvas.GetLeft(image4) - bossSpeed);
                    }

                    if (Canvas.GetTop(image4) > Canvas.GetTop(player) && Canvas.GetTop(image4) + image4.Height + 120 < myCanvas.Height && obezdvijivanie == false)
                    {
                        Canvas.SetTop(image4, Canvas.GetTop(image4) + bossSpeed);
                    }

                    if (Canvas.GetTop(image4) < Canvas.GetTop(player) && Canvas.GetTop(image4) > 80 && obezdvijivanie == false)
                    {
                        Canvas.SetTop(image4, Canvas.GetTop(image4) - bossSpeed);
                    }
                }


                foreach (UIElement j in elementsCopy)
                {

                    if ((j is Image image2 && ((string)image2.Tag == "bullet" || (string)image2.Tag == "sword" || (string)image2.Tag == "sphere")) && u is Image image3 && (string)image3.Tag == "boss") //Убийство мобов
                    {
                        if (Canvas.GetLeft(image3) < Canvas.GetLeft(image2) + image2.ActualWidth &&
                        Canvas.GetLeft(image3) + image3.ActualWidth > Canvas.GetLeft(image2) &&
                        Canvas.GetTop(image3) < Canvas.GetTop(image2) + image2.ActualHeight &&
                        Canvas.GetTop(image3) + image3.ActualHeight > Canvas.GetTop(image2))
                        {

                            int damage = 0;
                            if ((string)image2.Tag == "sphere")
                            {
                                damage = 1000;
                                if (foxyball == true)
                                {
                                    image3.Source = new BitmapImage(new Uri("charecter\\afk.png", UriKind.RelativeOrAbsolute));
                                    image3.Tag = null;
                                    disTimer.Stop();
                                    disbaniTimer.Stop();
                                    shootbaniTimer.Stop();
                                    shootTimer.Stop();

                                    DispatcherTimer poisonTimer = new DispatcherTimer();
                                    poisonTimer.Interval = TimeSpan.FromSeconds(1);
                                    int poisonDamageCount = 0;
                                    poisonTimer.Tick += (sender, e) =>
                                    {
                                        poisonDamageCount++;
                                        if (poisonDamageCount >= 3)
                                        {
                                            if (BossBani)
                                            {
                                                image3.Source = new BitmapImage(new Uri("bosban.png", UriKind.RelativeOrAbsolute));
                                            }
                                            else if (BossKyhnya)
                                            {
                                                image3.Source = new BitmapImage(new Uri("Босс1.png", UriKind.RelativeOrAbsolute));
                                            }
                                            else if (BossKotelnaya)
                                            {
                                                image3.Source = new BitmapImage(new Uri("boss1.png", UriKind.RelativeOrAbsolute));
                                            }

                                            image3.Tag = "boss";
                                            poisonTimer.Stop();
                                            disTimer.Start();
                                            disbaniTimer.Start();
                                            shootbaniTimer.Start();
                                            shootTimer.Start();
                                        }
                                    };
                                    poisonTimer.Start();
                                }
                            }


                            else if ((string)image2.Tag == "sword")
                            {
                                damage = 25;
                                if (poisonsworf == true)
                                {
                                    DispatcherTimer poisonTimer = new DispatcherTimer();
                                    poisonTimer.Interval = TimeSpan.FromSeconds(1);
                                    int poisonDamageCount = 0;
                                    poisonTimer.Tick += (sender, e) =>
                                    {
                                        bossHealthBar.Value -= 25;
                                        poisonDamageCount++;

                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            if (BossBani)
                                            {
                                                image3.Source = new BitmapImage(new Uri("charecter\\bosbanp.png", UriKind.RelativeOrAbsolute));
                                            }
                                            else if (BossKyhnya)
                                            {
                                                image3.Source = new BitmapImage(new Uri("charecter\\boskyhp.png", UriKind.RelativeOrAbsolute));
                                            }
                                            else if (BossKotelnaya)
                                            {
                                                image3.Source = new BitmapImage(new Uri("charecter\\boskotp.png", UriKind.RelativeOrAbsolute));
                                            }
                                        });

                                        if (bossHealthBar.Value < 1)
                                        {
                                            myCanvas.Children.Remove(image3);
                                            image3.Source = null;
                                            myCanvas.Children.Remove(bossHealthBar);

                                            door1.Visibility = Visibility.Visible;
                                            chest.Visibility = Visibility.Visible;

                                            poisonTimer.Stop();
                                        }
                                        else if (poisonDamageCount >= 3)
                                        {
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                if (BossBani)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("bosban.png", UriKind.RelativeOrAbsolute));
                                                }
                                                else if (BossKyhnya)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("Босс1.png", UriKind.RelativeOrAbsolute));
                                                }
                                                else if (BossKotelnaya)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("boss1.png", UriKind.RelativeOrAbsolute));
                                                }
                                            });
                                            poisonTimer.Stop();
                                        }
                                    };
                                    poisonTimer.Start();
                                }
                            }

                            else if ((string)image2.Tag == "bullet")
                            {
                                damage = 15;

                                if (bullet_ice)
                                {
                                    damage = 25;
                                    bossSpeed = 1;

                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        if (BossBani)
                                        {
                                            image3.Source = new BitmapImage(new Uri("charecter\\bosbanz.png", UriKind.RelativeOrAbsolute));
                                        }
                                        else if (BossKyhnya)
                                        {
                                            image3.Source = new BitmapImage(new Uri("charecter\\boskyhz.png", UriKind.RelativeOrAbsolute));
                                        }
                                        else if (BossKotelnaya)
                                        {
                                            image3.Source = new BitmapImage(new Uri("charecter\\boskotz.png", UriKind.RelativeOrAbsolute));
                                        }
                                    });

                                    if (freezeTimer == null)
                                    {
                                        freezeTimer = new System.Timers.Timer(3000);
                                        freezeTimer.Elapsed += (sender, e) =>
                                        {
                                            bossSpeed = 3;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                if (BossBani)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("bosban.png", UriKind.RelativeOrAbsolute));
                                                }
                                                else if (BossKyhnya)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("Босс1.png", UriKind.RelativeOrAbsolute));
                                                }
                                                else if (BossKotelnaya)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("boss1.png", UriKind.RelativeOrAbsolute));
                                                }
                                            });
                                            freezeTimer.Stop();
                                            freezeTimer = null;
                                        };
                                        freezeTimer.AutoReset = false;
                                        freezeTimer.Start();
                                    }
                                }
                            }

                            myCanvas.Children.Remove(image2);
                            image2.Source = null;

                            bossHealthBar.Value -= damage;

                            if (bossHealthBar.Value < 1)
                            {
                                myCanvas.Children.Remove(image3);
                                image3.Source = null;
                                myCanvas.Children.Remove(bossHealthBar);

                                door1.Visibility = Visibility.Visible;
                                chest.Visibility = Visibility.Visible;

                                if (BossKotelnaya)
                                {
                                    Image trof = new Image();
                                    trof.Source = new BitmapImage(new Uri("lut3.png", UriKind.RelativeOrAbsolute));
                                    trof.Tag = "trof1";
                                    trof.Height = 80;
                                    trof.Width = 80;

                                    Canvas.SetLeft(trof, Canvas.GetLeft(image3));
                                    Canvas.SetTop(trof, Canvas.GetTop(image3));

                                    myCanvas.Children.Add(trof);
                                }

                                if (BossKyhnya)
                                {
                                    Image trof = new Image();
                                    trof.Source = new BitmapImage(new Uri("lut1.png", UriKind.RelativeOrAbsolute));
                                    trof.Tag = "trof2";
                                    trof.Height = 80;
                                    trof.Width = 80;

                                    Canvas.SetLeft(trof, Canvas.GetLeft(image3));
                                    Canvas.SetTop(trof, Canvas.GetTop(image3));

                                    myCanvas.Children.Add(trof);
                                }

                                if (BossBani)
                                {
                                    Image trof = new Image();
                                    trof.Source = new BitmapImage(new Uri("lut2.png", UriKind.RelativeOrAbsolute));
                                    trof.Tag = "trof3";
                                    trof.Height = 80;
                                    trof.Width = 80;

                                    Canvas.SetLeft(trof, Canvas.GetLeft(image3));
                                    Canvas.SetTop(trof, Canvas.GetTop(image3));

                                    myCanvas.Children.Add(trof);
                                }
                            }

                        }


                    }
                }
            }
        }
    }
}
