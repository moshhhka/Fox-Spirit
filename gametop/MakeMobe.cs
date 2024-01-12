using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        Image stenka;
        Image door1;
        Canvas myCanvas;
        public List<UIElement> elementsCopy;

        public static bool MobeKyhnya, MobeKotelnaya, MobeBani;
        public static bool WasMobeKyhnya, WasMobeKotelnaya, WasMobeBani;
        public static List<int> chosenNumbers = new List<int>();
        public static Random randNumMobe = new Random();


        public ProgressBar zombieHealthBar;
        public static Dictionary<Image, ProgressBar> zombieBars = new Dictionary<Image, ProgressBar>();
        public Dictionary<Image, int> zombieSpeeds = new Dictionary<Image, int>();
        public static bool bullet_ice, poisonsworf, foxyball;

        System.Timers.Timer freezeTimer = null;

        public static DispatcherTimer disTimer = new DispatcherTimer();
        public static DispatcherTimer shootTimer = new DispatcherTimer();

        public MakeMobe(Image player, List<UIElement> elementsCopy, List<Image> zombieList, Canvas myCanvas, Image door1, Image stenka, int zombieSpeed = 3, int score = 0)
        {
            this.player = player;
            this.myCanvas = myCanvas;
            this.zombieSpeed = zombieSpeed;
            this.score = score;
            this.elementsCopy = elementsCopy;
            this.zombieList = zombieList;
            this.door1 = door1;
            this.stenka = stenka;
        }

        public void MakeZombies() 
        {
            Image zombie = new Image();
            zombie.Tag = "mobe";

            if (MobeBani)
            {
                zombie.Source = new BitmapImage(new Uri("charecter\\mobebani.png", UriKind.RelativeOrAbsolute));
            }
            else if (MobeKyhnya)
            {
                zombie.Source = new BitmapImage(new Uri("charecter\\bosskyhnya.png", UriKind.RelativeOrAbsolute));
            }
            else if (MobeKotelnaya)
            {
                zombie.Source = new BitmapImage(new Uri("charecter\\bos1et.png", UriKind.RelativeOrAbsolute));
            }


            double zombieLeft, zombieTop;
            do
            {
                zombieLeft = randNum.Next(0, 1595);
                zombieTop = randNum.Next(80, 780);
            }


            while (Math.Abs(Canvas.GetLeft(player) - zombieLeft) < 600 && Math.Abs(Canvas.GetTop(player) - zombieTop) < 600);

            Canvas.SetLeft(zombie, zombieLeft);
            Canvas.SetTop(zombie, zombieTop);

            zombie.Height = 296;
            zombie.Width = 302;
            zombieSpeeds[zombie] = 3;

            zombieList.Add(zombie);
            myCanvas.Children.Add(zombie);

            zombieHealthBar = new ProgressBar();
            zombieHealthBar.Width = 260;
            zombieHealthBar.Height = 20;
            zombieHealthBar.Value = 100;
            zombieHealthBar.Maximum = 100;
            Canvas.SetLeft(zombieHealthBar, zombieLeft);
            Canvas.SetTop(zombieHealthBar, zombieTop - zombieHealthBar.Height);
            myCanvas.Children.Add(zombieHealthBar); 
            zombieBars.Add(zombie, zombieHealthBar);


            Canvas.SetZIndex(player, 1);
            Canvas.SetZIndex(stenka, 1);

            shootTimer.Interval = TimeSpan.FromMilliseconds(2200);
            shootTimer.Tick += new EventHandler(shootTimerEvent);
            shootTimer.Start();

            disTimer.Interval = TimeSpan.FromMilliseconds(1000);
            disTimer.Tick += new EventHandler(disTimerEvent);
            disTimer.Start();
        }

        private void shootTimerEvent(object sender, EventArgs e)
        {
            if (!MobeKyhnya) return;

            foreach (UIElement u in elementsCopy)
            {
                if (Player.playerHealth <= 0)
                {
                    shootTimer.Stop();
                    return; 
                }

                if (u is Image image1 && (string)image1.Tag == "mobe" && zombieBars.ContainsKey(image1) && zombieBars[image1].Value > 0) //Движение мобов
                {
                    MobeBullet shootBullet = new MobeBullet();
                    string direction = CalculateDirection(Canvas.GetLeft(image1), Canvas.GetTop(image1), Canvas.GetLeft(player), Canvas.GetTop(player));
                    shootBullet.direction = direction;
                    shootBullet.bulletLeft = (int)Math.Round(Canvas.GetLeft(image1) + (image1.Width / 2));
                    shootBullet.bulletTop = (int)Math.Round(Canvas.GetTop(image1) + (image1.Height / 2));
                    string bulname = "mobebullet.png";
                    shootBullet.MakeMobeBullet(myCanvas, bulname);
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

        public async void disTimerEvent(object sender, EventArgs e)
        {
            if (!MobeBani) return;

            foreach (UIElement u in elementsCopy)
            {
                if (u is Image image1 && (string)image1.Tag == "mobe")
                {
                    int originalSpeed = zombieSpeeds[image1];

                    int delay = randNum.Next(1, 3) * 1000;

                    await Task.Delay(delay);

                    zombieSpeeds[image1] = 15;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        image1.Source = new BitmapImage(new Uri("charecter\\bany.png", UriKind.RelativeOrAbsolute));
                    });

                    DispatcherTimer speedResetTimer = new DispatcherTimer();
                    speedResetTimer.Interval = TimeSpan.FromSeconds(1);
                    speedResetTimer.Tick += (s, ev) =>
                    {
                        zombieSpeeds[image1] = originalSpeed;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            image1.Source = new BitmapImage(new Uri("charecter\\mobebani.png", UriKind.RelativeOrAbsolute));
                        });

                        speedResetTimer.Stop();
                    };
                    speedResetTimer.Start();
                }
            }
        }

        System.Timers.Timer timer = null;

        public void MoveMobe()
        {
            foreach (UIElement u in elementsCopy)
            {
                if (u is Image image1 && (string)image1.Tag == "mobe") 
                {
                    int zombieSpeed = zombieSpeeds[image1];

                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.RenderSize.Width, player.RenderSize.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image1), Canvas.GetTop(image1), image1.RenderSize.Width, image1.RenderSize.Height);

                    if (rect1.IntersectsWith(rect2) && (MobeBani || MobeKotelnaya))
                    {

                        if (timer == null)
                        {
                            timer = new System.Timers.Timer(500);
                            timer.Elapsed += (sender, e) =>
                            {
                                Player.playerHealth -= 2; 
                                timer.Stop(); 
                                timer = null;
                            };
                            timer.AutoReset = false; 
                            timer.Start(); 
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


                foreach (UIElement j in elementsCopy)
                {

                    if ((j is Image image2 && ((string)image2.Tag == "bullet" || (string)image2.Tag == "sword" || (string)image2.Tag == "sphere")) && u is Image image3 && (string)image3.Tag == "mobe") //Убийство мобов
                    {
                        Rect rect1 = new Rect(Canvas.GetLeft(image2), Canvas.GetTop(image2), image2.Width, image2.Height);
                        Rect rect2 = new Rect(Canvas.GetLeft(image3), Canvas.GetTop(image3), image3.Width, image3.Height);

                        if (rect1.IntersectsWith(rect2))
                        {
                            int damage = 0;
                            if ((string)image2.Tag == "sphere")
                            {
                                damage = 45;
                                if (foxyball == true)
                                {
                                    image3.Source = new BitmapImage(new Uri("charecter\\afk.png", UriKind.RelativeOrAbsolute));
                                    image3.Tag = null;
                                    disTimer.Stop();
                                    shootTimer.Stop();

                                    DispatcherTimer poisonTimer = new DispatcherTimer();
                                    poisonTimer.Interval = TimeSpan.FromSeconds(1);
                                    int poisonDamageCount = 0;
                                    poisonTimer.Tick += (sender, e) =>
                                    {
                                        poisonDamageCount++;
                                        if (poisonDamageCount >= 3)
                                        {
                                            if (MobeBani)
                                            {
                                                image3.Source = new BitmapImage(new Uri("charecter\\mobebani.png", UriKind.RelativeOrAbsolute));
                                            }
                                            else if (MobeKyhnya)
                                            {
                                                image3.Source = new BitmapImage(new Uri("charecter\\bosskyhnya.png", UriKind.RelativeOrAbsolute));
                                            }
                                            else if (MobeKotelnaya)
                                            {
                                                image3.Source = new BitmapImage(new Uri("charecter\\bos1et.png", UriKind.RelativeOrAbsolute));
                                            }

                                            image3.Tag = "mobe";
                                            poisonTimer.Stop();
                                            disTimer.Start();
                                            shootTimer.Start();
                                        }
                                    };
                                    poisonTimer.Start();
                                }
                            }

                            else if ((string)image2.Tag == "sword")
                            {
                                damage = 30;
                                if (poisonsworf == true)
                                {
                                    DispatcherTimer poisonTimer = new DispatcherTimer();
                                    poisonTimer.Interval = TimeSpan.FromSeconds(1);
                                    int poisonDamageCount = 0;
                                    poisonTimer.Tick += (sender, e) =>
                                    {
                                        if (zombieBars.ContainsKey(image3))
                                        {
                                            ProgressBar zombieHealthBar = zombieBars[image3];
                                            zombieHealthBar.Value -= 5;
                                            poisonDamageCount++;

                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                if (MobeBani)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("charecter\\banp.png", UriKind.RelativeOrAbsolute));
                                                }
                                                else if (MobeKyhnya)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("charecter\\kyhp.png", UriKind.RelativeOrAbsolute));
                                                }
                                                else if (MobeKotelnaya)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("charecter\\kotp.png", UriKind.RelativeOrAbsolute));
                                                }
                                            });

                                            if (zombieHealthBar.Value < 1)
                                            {
                                                myCanvas.Children.Remove(image3);
                                                image3.Source = null;
                                                zombieList.Remove(image3);
                                                myCanvas.Children.Remove(zombieHealthBar);
                                                zombieBars.Remove(image3);
                                                score++;
                                                if (score <= 12)
                                                {
                                                    MakeZombies();
                                                }
                                                if (score == 15)
                                                {
                                                    door1.Visibility = Visibility.Visible;
                                                }
                                                poisonTimer.Stop();
                                            }
                                        }
                                        else if (poisonDamageCount >= 3)
                                        {
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                if (MobeBani)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("charecter\\mobebani.png", UriKind.RelativeOrAbsolute));
                                                }
                                                else if (MobeKyhnya)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("charecter\\bosskyhnya.png", UriKind.RelativeOrAbsolute));
                                                }
                                                else if (MobeKotelnaya)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("charecter\\bos1et.png", UriKind.RelativeOrAbsolute));
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
                                    zombieSpeeds[image3] = 1;
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        if (MobeBani)
                                        {
                                            image3.Source = new BitmapImage(new Uri("charecter\\banz.png", UriKind.RelativeOrAbsolute));
                                        }
                                        else if (MobeKyhnya)
                                        {
                                            image3.Source = new BitmapImage(new Uri("charecter\\kyhz.png", UriKind.RelativeOrAbsolute));
                                        }
                                        else if (MobeKotelnaya)
                                        {
                                            image3.Source = new BitmapImage(new Uri("charecter\\kotz.png", UriKind.RelativeOrAbsolute));
                                        }
                                    });

                                    if (freezeTimer == null)
                                    {
                                        freezeTimer = new System.Timers.Timer(3000);
                                        freezeTimer.Elapsed += (sender, e) =>
                                        {
                                            zombieSpeeds[image3] = 3;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                if (MobeBani)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("charecter\\mobebani.png", UriKind.RelativeOrAbsolute));
                                                }
                                                else if (MobeKyhnya)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("charecter\\bosskyhnya.png", UriKind.RelativeOrAbsolute));
                                                }
                                                else if (MobeKotelnaya)
                                                {
                                                    image3.Source = new BitmapImage(new Uri("charecter\\bos1et.png", UriKind.RelativeOrAbsolute));
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


                            if (zombieBars.ContainsKey(image3))
                            {
                                ProgressBar zombieHealthBar = zombieBars[image3];
                                zombieHealthBar.Value -= damage;
                                if (zombieHealthBar.Value < 1)
                                {
                                    double mobeLeft = Canvas.GetLeft(image3);
                                    double mobeTop = Canvas.GetTop(image3);

                                    myCanvas.Children.Remove(image3);
                                    image3.Source = null;
                                    zombieList.Remove(image3);
                                    myCanvas.Children.Remove(zombieHealthBar);
                                    zombieBars.Remove(image3);
                                    score++;

                                    if (MobeBani)
                                    {
                                        MobeHitSpace hitSpace = new MobeHitSpace();
                                        hitSpace.MakeSphere(myCanvas, player);
                                        Canvas.SetLeft(hitSpace.sphere, mobeLeft);
                                        Canvas.SetTop(hitSpace.sphere, mobeTop);

                                        DispatcherTimer sphereTimer = new DispatcherTimer();
                                        sphereTimer.Interval = TimeSpan.FromSeconds(5);
                                        sphereTimer.Tick += (s, e) =>
                                        {
                                            myCanvas.Children.Remove(hitSpace.sphere);
                                            sphereTimer.Stop();
                                        };
                                        sphereTimer.Start();
                                    }

                                    double randomNumber = randNum.NextDouble();


                                    if (randomNumber < 0.3)
                                    {
                                        Image coffee = new Image();
                                        coffee.Tag = "heal";
                                        coffee.Height = 60;
                                        coffee.Width = 60;
                                        coffee.Source = new BitmapImage(new Uri("heal.png", UriKind.RelativeOrAbsolute));
                                        Canvas.SetLeft(coffee, Canvas.GetLeft(image3));
                                        Canvas.SetTop(coffee, Canvas.GetTop(image3));
                                        myCanvas.Children.Add(coffee);
                                    }

                                    if (score <= 12)
                                    {
                                        MakeZombies();
                                    }
                                    if (score == 15)
                                    {
                                        door1.Visibility = Visibility.Visible;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
