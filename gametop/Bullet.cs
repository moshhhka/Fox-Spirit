using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace gametop
{
    internal class Bullet
    {
        public string direction;
        public int bulletLeft;
        public int bulletTop;

        private int speed = 20;
        private Image bullet = new Image();
        List<Bullet> bullets;

        public Bullet(List<Bullet> bullets)
        {
            this.bullets = bullets;
        }


        public void MakeBullet(Canvas form)
        {
            bullet.Source = new BitmapImage(new Uri("plbul.png", UriKind.RelativeOrAbsolute));
            bullet.Height = 80;
            bullet.Width = 80;
            bullet.Tag = "bullet";
            Canvas.SetLeft(bullet, bulletLeft);
            Canvas.SetTop(bullet, bulletTop);
            Canvas.SetZIndex(bullet, 1);

            form.Children.Add(bullet);
        }

        


        public void BulletMove()
        { 
            if (direction == "left")
            {
                Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) - speed);
            }

            if (direction == "right")
            {
                Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) + speed);
            }

            if (direction == "up")
            {
                Canvas.SetTop(bullet, Canvas.GetTop(bullet) - speed);
            }

            if (direction == "down")
            {
                Canvas.SetTop(bullet, Canvas.GetTop(bullet) + speed);
            }

            if (Canvas.GetLeft(bullet) < 10 || Canvas.GetLeft(bullet) > 1800 || Canvas.GetTop(bullet) < 10 || Canvas.GetTop(bullet) > 980)
            {
                bullet.Source = null;
                bullets.Remove(this);
            }

        }
    }
}
