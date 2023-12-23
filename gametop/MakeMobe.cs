using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace gametop
{
    internal class MakeMobe
    {

        List<Image> zombieList;
        public int zombieSpeed, score;
        Random randNum = new Random();
        Image player;
        Image key;
        Image chest;
        Canvas myCanvas;
        public List<UIElement> elementsCopy;
        private ProgressBar zombieHealthBar;


        Dictionary<Image, ProgressBar> zombieBars = new Dictionary<Image, ProgressBar>();

        public MakeMobe(Image player, List<UIElement> elementsCopy, List<Image> zombieList, Canvas myCanvas, Image key, Image chest, int zombieSpeed = 3, int score = 0)
        {
            this.player = player;
            this.myCanvas = myCanvas;
            this.zombieSpeed = zombieSpeed;
            this.score = score;
            this.elementsCopy = elementsCopy;
            this.zombieList = zombieList;
            this.key = key;
            this.chest = chest;
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
        }

        System.Timers.Timer timer = null;

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

                foreach (UIElement j in elementsCopy)
                {
                   
                    if ((j is Image image2 && ((string)image2.Tag == "bullet" || (string)image2.Tag == "sword" || (string)image2.Tag == "sphere")) && u is Image image3 && (string)image3.Tag == "mobe") //Убийство мобов
                    {
                        if (Canvas.GetLeft(image3) < Canvas.GetLeft(image2) + image2.ActualWidth &&
                        Canvas.GetLeft(image3) + image3.ActualWidth > Canvas.GetLeft(image2) &&
                        Canvas.GetTop(image3) < Canvas.GetTop(image2) + image2.ActualHeight &&
                        Canvas.GetTop(image3) + image3.ActualHeight > Canvas.GetTop(image2))
                        {

                            int damage = (string)image2.Tag == "sphere" ? 50 : 30;
                            if ((string)image2.Tag != "sphere") // Если это не sphere, удаляем сразу
                            {
                                myCanvas.Children.Remove(image2);
                                image2.Source = null;
                            }

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

                                if (score <= 12)
                                {
                                    MakeZombies();
                                }

                                if (score == 15)
                                {
                                    key.Visibility = Visibility.Visible;
                                    chest.Visibility = Visibility.Visible;
                                }
                            }

                        }
                    }
                }
            }
        }
    }
}
