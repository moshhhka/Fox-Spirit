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
        public static int bossHealth = 2000;
        ProgressBar bossHealthBar;

        public static bool bullet_ice, poisonsworf, foxyball;

        // Добавьте новый таймер для восстановления скорости зомби после замораживания
        System.Timers.Timer freezeTimer = null;

        public static DispatcherTimer disTimer = new DispatcherTimer();

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
            
        }

        public void MakeBoss1()
        {
            Image boss = new Image();
            boss.Source = new BitmapImage(new Uri("boss1.png", UriKind.RelativeOrAbsolute));
            boss.Tag = "boss";
            boss.Height = 408;
            boss.Width = 300;

            Canvas.SetLeft(boss, 810);
            Canvas.SetTop(boss, 336);

            bossHealthBar = new ProgressBar();
            bossHealthBar.Height = 40;
            bossHealthBar.Width = 1500;
            bossHealthBar.Maximum = 2000;
            bossHealthBar.Value = bossHealth;

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
            if (bossHealth > 1)
            {
                bossHealthBar.Value = bossHealth;
            }

            foreach (UIElement u in elementsCopy)
            {
                if (u is Image image1 && (string)image1.Tag == "boss") //Движение мобов
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
                                damage = 15;
                                if (foxyball == true)
                                {
                                    image3.Source = new BitmapImage(new Uri("charecter\\afk.png", UriKind.RelativeOrAbsolute));
                                    image3.Tag = null;
                                    disTimer.Stop();

                                    DispatcherTimer poisonTimer = new DispatcherTimer();
                                    poisonTimer.Interval = TimeSpan.FromSeconds(1);
                                    int poisonDamageCount = 0;
                                    poisonTimer.Tick += (sender, e) =>
                                    {
                                        poisonDamageCount++;
                                        if (poisonDamageCount >= 3)
                                        {
                                            image3.Source = new BitmapImage(new Uri("boss1.png", UriKind.RelativeOrAbsolute));
                                            image3.Tag = "boss";
                                            poisonTimer.Stop();
                                            disTimer.Start();
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
                                        bossHealth -= 25;
                                        poisonDamageCount++;

                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            image3.Source = new BitmapImage(new Uri("charecter\\boskotp.png", UriKind.RelativeOrAbsolute));
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
                                                image3.Source = new BitmapImage(new Uri("boss1.png", UriKind.RelativeOrAbsolute));
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
                                        image3.Source = new BitmapImage(new Uri("charecter\\boskotz.png", UriKind.RelativeOrAbsolute));
                                    });

                                    if (freezeTimer == null)
                                    {
                                        freezeTimer = new System.Timers.Timer(3000);
                                        freezeTimer.Elapsed += (sender, e) =>
                                        {
                                            bossSpeed = 3;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                image3.Source = new BitmapImage(new Uri("boss1.png", UriKind.RelativeOrAbsolute));
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

                            bossHealth -= damage;
                            bossHealthBar.Value = bossHealth;

                            if (bossHealthBar.Value < 1)
                            {
                                myCanvas.Children.Remove(image3);
                                image3.Source = null;
                                myCanvas.Children.Remove(bossHealthBar);

                                door1.Visibility = Visibility.Visible;
                                chest.Visibility = Visibility.Visible;
                            }

                        }


                    }
                }
            }
        }
    }
}
