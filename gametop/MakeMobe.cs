using System;
using System.Collections.Generic;
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
        Image stenka;
        Image door1;
        Canvas myCanvas;
        public List<UIElement> elementsCopy;

        public ProgressBar zombieHealthBar;
        public static Dictionary<Image, ProgressBar> zombieBars = new Dictionary<Image, ProgressBar>();
        public Dictionary<Image, int> zombieSpeeds = new Dictionary<Image, int>();
        public static bool bullet_ice, poisonsworf, foxyball;

        // Добавьте новый таймер для восстановления скорости зомби после замораживания
        System.Timers.Timer freezeTimer = null;




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


        public void MakeZombies() // Создание мобов
        {

            Image zombie = new Image();
            zombie.Tag = "mobe";
            zombie.Source = new BitmapImage(new Uri("charecter\\bos1et.png", UriKind.RelativeOrAbsolute));

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
            myCanvas.Children.Add(zombieHealthBar); // Добавляем ProgressBar на Canvas
            zombieBars.Add(zombie, zombieHealthBar);


            Canvas.SetZIndex(player, 1);
            Canvas.SetZIndex(stenka, 1);
        }

        System.Timers.Timer timer = null;

        public void MoveMobe()
        {
            foreach (UIElement u in elementsCopy)
            {
                if (u is Image image1 && (string)image1.Tag == "mobe") //Движение мобов
                {
                    int zombieSpeed = zombieSpeeds[image1];

                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.RenderSize.Width, player.RenderSize.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image1), Canvas.GetTop(image1), image1.RenderSize.Width, image1.RenderSize.Height);

                    if (rect1.IntersectsWith(rect2))
                    {

                        if (timer == null)
                        {
                            timer = new System.Timers.Timer(500);
                            timer.Elapsed += (sender, e) =>
                            {
                                Player.playerHealth -= 3; // Уменьшите здоровье на 5 через секунду
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


                foreach (UIElement j in elementsCopy)
                {

                    if ((j is Image image2 && ((string)image2.Tag == "bullet" || (string)image2.Tag == "sword" || (string)image2.Tag == "sphere")) && u is Image image3 && (string)image3.Tag == "mobe") //Убийство мобов
                    {
                        Rect rect1 = new Rect(Canvas.GetLeft(image2), Canvas.GetTop(image2), image2.Width, image2.Height);
                        Rect rect2 = new Rect(Canvas.GetLeft(image3), Canvas.GetTop(image3), image3.Width, image3.Height);

                        if (rect1.IntersectsWith(rect2))
                        {

                            // Теперь вы можете использовать zombieHealthBar
                            int damage = 0;
                            if ((string)image2.Tag == "sphere")
                            {
                                damage = 15;
                                if (foxyball == true)
                                {
                                    image3.Source = new BitmapImage(new Uri("charecter\\afk.png", UriKind.RelativeOrAbsolute));
                                    image3.Tag = null;

                                    DispatcherTimer poisonTimer = new DispatcherTimer();
                                    poisonTimer.Interval = TimeSpan.FromSeconds(1);
                                    int poisonDamageCount = 0;
                                    poisonTimer.Tick += (sender, e) =>
                                    {
                                        poisonDamageCount++;
                                        if (poisonDamageCount >= 3)
                                        {
                                            image3.Source = new BitmapImage(new Uri("charecter\\bos1et.png", UriKind.RelativeOrAbsolute));
                                            image3.Tag = "mobe";
                                            poisonTimer.Stop();
                                        }
                                    };
                                    poisonTimer.Start();
                                }
                            }


                            else if ((string)image2.Tag == "sword")
                            {
                                damage = 15;
                                if (poisonsworf == true)
                                {
                                    DispatcherTimer poisonTimer = new DispatcherTimer();
                                    poisonTimer.Interval = TimeSpan.FromSeconds(1);
                                    int poisonDamageCount = 0;
                                    poisonTimer.Tick += (sender, e) =>
                                    {
                                        ProgressBar zombieHealthBar = zombieBars[image3];
                                        zombieHealthBar.Value -= 5;
                                        poisonDamageCount++;
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
                                        else if (poisonDamageCount >= 3)
                                        {
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
                                    
                                    if (freezeTimer == null)
                                    {
                                        freezeTimer = new System.Timers.Timer(3000);
                                        freezeTimer.Elapsed += (sender, e) =>
                                        {
                                            zombieSpeeds[image3] = 3; 
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
                                    myCanvas.Children.Remove(image3);
                                    image3.Source = null;
                                    zombieList.Remove(image3);
                                    myCanvas.Children.Remove(zombieHealthBar);
                                    zombieBars.Remove(image3);
                                    score++;


                                    double randomNumber = randNum.NextDouble();

                                    
                                    if (randomNumber < 0.35)
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
