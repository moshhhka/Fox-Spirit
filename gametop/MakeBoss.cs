using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows;

namespace gametop
{
    internal class MakeBoss
    {

        public string direction;
        Image player;
        Image boss;
        Image key;
        Canvas myCanvas;
        public List<UIElement> elementsCopy;
        public static int bossSpeed = 2;
        public static int bossHealth = 1000;
        double bossLeft, bossTop;
        ProgressBar bossHealthBar;

        DispatcherTimer shootTimer = new DispatcherTimer();


        public MakeBoss(Image player, List<UIElement> elementsCopy, Canvas myCanvas, Image key, ProgressBar bossHealthBar, Image boss)
        {
            this.player = player;
            this.myCanvas = myCanvas;
            this.elementsCopy = elementsCopy;
            this.key = key;
            this.bossHealthBar = bossHealthBar;
            this.boss = boss;

            shootTimer.Interval = TimeSpan.FromMilliseconds(1800);
            shootTimer.Tick += new EventHandler(shootTimerEvent);
            shootTimer.Start();

            bossLeft = Canvas.GetLeft(boss);
            bossTop = Canvas.GetTop(boss);
        }


        System.Timers.Timer timer = null;

        private void shootTimerEvent(object sender, EventArgs e)
        {
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
                    MobeBullet shootBullet = new MobeBullet();
                    shootBullet.direction = direction;
                    shootBullet.bulletLeft = (int)Math.Round(Canvas.GetLeft(image1) + (image1.Width / 2));
                    shootBullet.bulletTop = (int)Math.Round(Canvas.GetTop(image1) + (image1.Height / 2));
                    shootBullet.MakeMobeBullet(myCanvas);
                }
            }
        }

        public string CalculateDirection(double bossLeft, double bossTop, double playerLeft, double playerTop)
        {
            // Вычислите разницу между координатами зомби и игрока
            double deltaX = playerLeft - bossLeft;
            double deltaY = playerTop - bossTop;

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
                                // Предположим, что у вас есть экземпляр класса HitSpace с именем hitSpace
                                HitSpace hitSpace = new HitSpace();

                                // Теперь вы можете вызвать метод ApplySphereDamage() через этот экземпляр
                                hitSpace.ApplySphereDamage();
                                damage = 10;
                                
                            }

                            else if ((string)image2.Tag == "sword")
                            {
                                damage = 25;
                            }
                            else if ((string)image2.Tag == "bullet")
                            {
                                damage = 15;
                            }

                            if ((string)image2.Tag != "sphere") // Если это не sphere, удаляем сразу
                            {
                                myCanvas.Children.Remove(image2);
                                image2.Source = null;
                            }

                            bossHealth -= damage;
                            bossHealthBar.Value = bossHealth;

                            if (bossHealthBar.Value < 1)
                            {
                                myCanvas.Children.Remove(image3);
                                image3.Source = null;
                                myCanvas.Children.Remove(bossHealthBar);

                                key.Visibility = Visibility.Visible;
                                
                            }

                        }
                    }
                }
            }
        }
    }
}
