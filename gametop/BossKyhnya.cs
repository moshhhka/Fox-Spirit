using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace gametop
{
    internal class BossKyhnya
    {

        public string direction;
        Image player;
        Image boss;
        Image door1;
        Image chest;
        Canvas myCanvas;
        public List<UIElement> elementsCopy;
        public static int bossSpeed = 2;
        public static int bossHealth = 2000;
        double bossLeft, bossTop;
        ProgressBar bossHealthBar;

        public static bool bullet_ice, poisonsworf;

        // Добавьте новый таймер для восстановления скорости зомби после замораживания
        System.Timers.Timer freezeTimer = null;

        public static DispatcherTimer shootTimer = new DispatcherTimer();

        public BossKyhnya(Image player, List<UIElement> elementsCopy, Canvas myCanvas, Image door1, Image chest, ProgressBar bossHealthBar, Image boss)
        {
            this.player = player;
            this.myCanvas = myCanvas;
            this.elementsCopy = elementsCopy;
            this.door1 = door1;
            this.bossHealthBar = bossHealthBar;
            this.boss = boss;
            this.chest = chest;

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
                    // Определите все возможные направления
                    string[] directions = new string[] { "up", "down", "left", "right", "upleft", "upright", "downleft", "downright" };

                    // Создайте пулю в каждом направлении
                    foreach (string direction in directions)
                    {
                        MobeBullet shootBullet = new MobeBullet();
                        shootBullet.direction = direction;
                        shootBullet.bulletLeft = (int)Math.Round(Canvas.GetLeft(image1) + (image1.Width / 2));
                        shootBullet.bulletTop = (int)Math.Round(Canvas.GetTop(image1) + (image1.Height / 2));
                        shootBullet.MakeMobeBullet(myCanvas);
                    }
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
                                //if (poisonsworf == true)
                                //{
                                    image3.Source = new BitmapImage(new Uri("charecter\\afk.png", UriKind.RelativeOrAbsolute));
                                    image3.Tag = null;
                                    shootTimer.Stop();

                                    DispatcherTimer poisonTimer = new DispatcherTimer();
                                    poisonTimer.Interval = TimeSpan.FromSeconds(1);
                                    int poisonDamageCount = 0;
                                    poisonTimer.Tick += (sender, e) =>
                                    {
                                        poisonDamageCount++;
                                        if (poisonDamageCount >= 3)
                                        {
                                            image3.Source = new BitmapImage(new Uri("Босс1.png", UriKind.RelativeOrAbsolute));
                                            image3.Tag = "boss";
                                            poisonTimer.Stop();
                                            shootTimer.Start();
                                        }
                                    };
                                    poisonTimer.Start();
                                //}
                            }


                            else if ((string)image2.Tag == "sword")
                            {
                                damage = 25;
                                //if (poisonsworf == true)
                                //{
                                    DispatcherTimer poisonTimer = new DispatcherTimer();
                                    poisonTimer.Interval = TimeSpan.FromSeconds(1);
                                    int poisonDamageCount = 0;
                                    poisonTimer.Tick += (sender, e) =>
                                    {
                                        bossHealth -= 25;
                                        poisonDamageCount++;
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
                                            poisonTimer.Stop();
                                        }
                                    };
                                    poisonTimer.Start();
                                //}
                            }

                            else if ((string)image2.Tag == "bullet")
                            {
                                //damage = 15;

                                //if (bullet_ice)
                                //{
                                    damage = 25;
                                    bossSpeed = 1;

                                    if (freezeTimer == null)
                                    {
                                        freezeTimer = new System.Timers.Timer(3000);
                                        freezeTimer.Elapsed += (sender, e) =>
                                        {
                                            bossSpeed = 3;
                                            freezeTimer.Stop();
                                            freezeTimer = null;
                                        };
                                        freezeTimer.AutoReset = false;
                                        freezeTimer.Start();
                                    }
                                //}
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
