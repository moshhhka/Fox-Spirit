using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace gametop
{
    internal class MakeMobe
    {

        List<Image> zombieList;
        public int zombieSpeed, score;
        public string direction;
        Random randNum = new Random();
        Image player;
        Image key;
        Canvas myCanvas;
        public List<UIElement> elementsCopy;
        private ProgressBar zombieHealthBar;


        Dictionary<Image, ProgressBar> zombieBars = new Dictionary<Image, ProgressBar>();
        DispatcherTimer shootTimer = new DispatcherTimer();


        public MakeMobe(Image player, List<UIElement> elementsCopy, List<Image> zombieList, Canvas myCanvas, Image key, int zombieSpeed = 3, int score = 0)
        {
            this.player = player;
            this.myCanvas = myCanvas;
            this.zombieSpeed = zombieSpeed;
            this.score = score;
            this.elementsCopy = elementsCopy;
            this.zombieList = zombieList;
            this.key = key;
        }


        public void MakeZombies() // Создание мобов
        {
            Image zombie = new Image();
            zombie.Tag = "mobe";
            zombie.Source = new BitmapImage(new Uri("charecter\\bos1et.png", UriKind.RelativeOrAbsolute));

            // Генерируем случайные координаты для зомби
            double zombieLeft, zombieTop;
            do
            {
                zombieLeft = randNum.Next(0, 1595);
                zombieTop = randNum.Next(80, 780);
            }
            // Проверяем, что зомби не появляется слишком близко к игроку
            while (Math.Abs(Canvas.GetLeft(player) - zombieLeft) < 600 && Math.Abs(Canvas.GetTop(player) - zombieTop) < 600);

            Canvas.SetLeft(zombie, zombieLeft);
            Canvas.SetTop(zombie, zombieTop);

            zombie.Height = 296;
            zombie.Width = 302;
            zombieList.Add(zombie);
            myCanvas.Children.Add(zombie);

            zombieHealthBar = new ProgressBar();
            zombieHealthBar.Width = 260;
            zombieHealthBar.Height = 20;
            zombieHealthBar.Value = 100; // Устанавливаем здоровье зомби
            zombieHealthBar.Maximum = 100; // Устанавливаем максимальное значение ProgressBar

            // Размещаем ProgressBar над зомби
            Canvas.SetLeft(zombieHealthBar, zombieLeft);
            Canvas.SetTop(zombieHealthBar, zombieTop - zombieHealthBar.Height);

            myCanvas.Children.Add(zombieHealthBar); // Добавляем ProgressBar на Canvas

            zombieBars.Add(zombie, zombieHealthBar);

            Canvas.SetZIndex(player, 1);

            shootTimer.Interval = TimeSpan.FromMilliseconds(1500);
            shootTimer.Tick += new EventHandler(shootTimerEvent);
            shootTimer.Start();
        }

        System.Timers.Timer timer = null;

        private void shootTimerEvent(object sender, EventArgs e)
        {
            foreach (UIElement u in elementsCopy)
            {
                if (u is Image image1 && (string)image1.Tag == "mobe") //Движение мобов
                {
                    string direction = CalculateDirection(Canvas.GetLeft(image1), Canvas.GetTop(image1), Canvas.GetLeft(player), Canvas.GetTop(player));
                    Bullet shootBullet = new Bullet();
                    shootBullet.direction = direction;
                    shootBullet.bulletLeft = (int)Math.Round(Canvas.GetLeft(image1) + (image1.Width / 2));
                    shootBullet.bulletTop = (int)Math.Round(Canvas.GetTop(image1) + (image1.Height / 2));
                    shootBullet.MakeBullet(myCanvas);
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


        public void MoveMobe()
        {
            foreach (UIElement u in elementsCopy)
            {
                if (u is Image image1 && (string)image1.Tag == "mobe") //Движение мобов
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
                                Player.playerHealth -= 5; // Уменьшите здоровье на 5 через секунду
                                timer.Stop(); // Остановите таймер
                                timer = null; // Установите таймер в null
                            };
                            timer.AutoReset = false; // Установите AutoReset в false, чтобы таймер сработал только один раз
                            timer.Start(); // Запустите таймер
                        }
                    }


                    if (Canvas.GetLeft(image1) > Canvas.GetLeft(player))
                    {
                        Canvas.SetLeft(image1, Canvas.GetLeft(image1) - zombieSpeed);
                        ProgressBar zombieHealthBar = zombieBars[image1];
                        Canvas.SetLeft(zombieHealthBar, Canvas.GetLeft(image1));
                    }

                    if (Canvas.GetLeft(image1) < Canvas.GetLeft(player))
                    {
                        Canvas.SetLeft(image1, Canvas.GetLeft(image1) + zombieSpeed);
                        ProgressBar zombieHealthBar = zombieBars[image1];
                        Canvas.SetLeft(zombieHealthBar, Canvas.GetLeft(image1));
                    }

                    if (Canvas.GetTop(image1) > Canvas.GetTop(player))
                    {
                        Canvas.SetTop(image1, Canvas.GetTop(image1) - zombieSpeed);
                        ProgressBar zombieHealthBar = zombieBars[image1];
                        Canvas.SetTop(zombieHealthBar, Canvas.GetTop(image1) - zombieHealthBar.Height);
                    }

                    if (Canvas.GetTop(image1) < Canvas.GetTop(player))
                    {
                        Canvas.SetTop(image1, Canvas.GetTop(image1) + zombieSpeed);
                        ProgressBar zombieHealthBar = zombieBars[image1];
                        Canvas.SetTop(zombieHealthBar, Canvas.GetTop(image1) - zombieHealthBar.Height);
                    }

                }


                //foreach (UIElement j in elementsCopy)
                //{

                //    if ((j is Image image2 && ((string)image2.Tag == "bullet" || (string)image2.Tag == "sword" || (string)image2.Tag == "sphere")) && u is Image image3 && (string)image3.Tag == "mobe") //Убийство мобов
                //    {
                //        if (Canvas.GetLeft(image3) < Canvas.GetLeft(image2) + image2.ActualWidth &&
                //        Canvas.GetLeft(image3) + image3.ActualWidth > Canvas.GetLeft(image2) &&
                //        Canvas.GetTop(image3) < Canvas.GetTop(image2) + image2.ActualHeight &&
                //        Canvas.GetTop(image3) + image3.ActualHeight > Canvas.GetTop(image2))
                //        {

                //            ProgressBar zombieHealthBar = zombieBars[image3];

                //            int damage = 0;
                //            if ((string)image2.Tag == "sphere")
                //            {
                //                damage = 50;
                //            }
                //            else if ((string)image2.Tag == "sword")
                //            {
                //                damage = 35;
                //            }
                //            else if ((string)image2.Tag == "bullet")
                //            {
                //                damage = 15;
                //            }

                //            if ((string)image2.Tag != "sphere") // Если это не sphere, удаляем сразу
                //            {
                //                myCanvas.Children.Remove(image2);
                //                image2.Source = null;
                //            }

                            
                //            zombieHealthBar.Value -= damage;

                //            if (zombieHealthBar.Value < 1)
                //            {
                //                myCanvas.Children.Remove(image3);
                //                image3.Source = null;
                //                zombieList.Remove(image3);
                //                myCanvas.Children.Remove(zombieHealthBar);
                //                zombieBars.Remove(image3);
                //                score++;

                //                if (score <= 12)
                //                {
                //                    MakeZombies();
                //                }

                //                if (score == 15)
                //                {
                //                    key.Visibility = Visibility.Visible;
                //                }
                //            }

                //        }
                //    }
                //}
            }
        }


        
    }
}
