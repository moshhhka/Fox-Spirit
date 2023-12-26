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

namespace gametop
{
    internal class Bullet
    {
        public string direction;
        public int bulletLeft;
        public int bulletTop;

        private int speed = 20;
        private Image bullet = new Image();
        DispatcherTimer bulletTimer = new DispatcherTimer();



        public void MakeBullet(Canvas form)
        {
            bullet.Source = new BitmapImage(new Uri("playerbullet.png", UriKind.RelativeOrAbsolute));
            bullet.Height = 80;
            bullet.Width = 80;
            bullet.Tag = "bullet";
            Canvas.SetLeft(bullet, bulletLeft);
            Canvas.SetTop(bullet, bulletTop);
            Canvas.SetZIndex(bullet, 1);

            form.Children.Add(bullet);


            bulletTimer.Interval = TimeSpan.FromMilliseconds(speed);
            bulletTimer.Tick += new EventHandler(BulletTimerEvent);
            bulletTimer.Start();

        }


        private void BulletTimerEvent(object sender, EventArgs e)
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
                bulletTimer.Stop();
                bulletTimer.Stop();
                bullet.Source = null;
                bulletTimer = null;

            }

        }
    }
}
