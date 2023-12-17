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
            zombie.Source = new BitmapImage(new Uri("bos1et.png", UriKind.RelativeOrAbsolute));
            Canvas.SetLeft(zombie, randNum.Next(0, 1595));
            Canvas.SetTop(zombie, randNum.Next(80, 780));
            zombie.Height = 296;
            zombie.Width = 302;
            zombieList.Add(zombie);
            myCanvas.Children.Add(zombie);
            Canvas.SetZIndex(player, 1);
        }


        public void MoveMobe()
        {
            foreach (UIElement u in elementsCopy)
            {
                if (u is Image image1 && (string)image1.Tag == "mobe") //Движение мобов
                {
                    if (Canvas.GetLeft(player) < Canvas.GetLeft(image1) + image1.ActualWidth &&
                        Canvas.GetLeft(player) + player.ActualWidth > Canvas.GetLeft(image1) &&
                        Canvas.GetTop(player) < Canvas.GetTop(image1) + image1.ActualHeight &&
                        Canvas.GetTop(player) + player.ActualHeight > Canvas.GetTop(image1))
                    {
                        Player.playerHealth -= 1;
                    }

                    if (Canvas.GetLeft(image1) > Canvas.GetLeft(player))
                    {
                        Canvas.SetLeft(image1, Canvas.GetLeft(image1) - zombieSpeed);
                    }

                    if (Canvas.GetLeft(image1) < Canvas.GetLeft(player))
                    {
                        Canvas.SetLeft(image1, Canvas.GetLeft(image1) + zombieSpeed);
                    }

                    if (Canvas.GetTop(image1) > Canvas.GetTop(player))
                    {
                        Canvas.SetTop(image1, Canvas.GetTop(image1) - zombieSpeed);
                    }

                    if (Canvas.GetTop(image1) < Canvas.GetTop(player))
                    {
                        Canvas.SetTop(image1, Canvas.GetTop(image1) + zombieSpeed);
                    }
                }

                foreach (UIElement j in elementsCopy)
                {
                   
                    if (j is Image image2 && (string)image2.Tag == "bullet" && u is Image image3 && (string)image3.Tag == "mobe") //Убийство мобов
                    {
                        if (Canvas.GetLeft(image3) < Canvas.GetLeft(image2) + image2.ActualWidth &&
                        Canvas.GetLeft(image3) + image3.ActualWidth > Canvas.GetLeft(image2) &&
                        Canvas.GetTop(image3) < Canvas.GetTop(image2) + image2.ActualHeight &&
                        Canvas.GetTop(image3) + image3.ActualHeight > Canvas.GetTop(image2))
                        {
                            score++;

                            myCanvas.Children.Remove(image2);
                            image2.Source = null;
                            myCanvas.Children.Remove(image3);
                            image3.Source = null;
                            zombieList.Remove(image3);

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
