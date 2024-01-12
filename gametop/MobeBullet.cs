using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace gametop
{
    internal class MobeBullet
    {
        public string direction;
        public int bulletLeft;
        public int bulletTop;

        private int speed = 20;
        public Image bullet = new Image();
        public DispatcherTimer bulletTimer = new DispatcherTimer();
        Canvas form;


        public void MakeMobeBullet(Canvas form, string imageSource)
        {
            this.form = form;


            bulletTimer.Interval = TimeSpan.FromMilliseconds(speed);
            bulletTimer.Tick += new EventHandler(MobeBulletTimerEvent);
            bulletTimer.Start();

            bullet.Source = new BitmapImage(new Uri(imageSource, UriKind.RelativeOrAbsolute));
            bullet.Height = 80;
            bullet.Width = 80;
            bullet.Tag = "mobebullet";
            Canvas.SetLeft(bullet, bulletLeft);
            Canvas.SetTop(bullet, bulletTop);
            Canvas.SetZIndex(bullet, 1);

            bullet.DataContext = this;

            form.Children.Add(bullet);
        }

        public void MobeBulletTimerEvent(object sender, EventArgs e)
        {
            switch (direction)
            {
                case "left":
                    Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) - speed);
                    break;
                case "right":
                    Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) + speed);
                    break;
                case "up":
                    Canvas.SetTop(bullet, Canvas.GetTop(bullet) - speed);
                    break;
                case "down":
                    Canvas.SetTop(bullet, Canvas.GetTop(bullet) + speed);
                    break;
                case "upleft":
                    Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) - speed);
                    Canvas.SetTop(bullet, Canvas.GetTop(bullet) - speed);
                    break;
                case "upright":
                    Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) + speed);
                    Canvas.SetTop(bullet, Canvas.GetTop(bullet) - speed);
                    break;
                case "downleft":
                    Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) - speed);
                    Canvas.SetTop(bullet, Canvas.GetTop(bullet) + speed);
                    break;
                case "downright":
                    Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) + speed);
                    Canvas.SetTop(bullet, Canvas.GetTop(bullet) + speed);
                    break;
            }


            if (Canvas.GetLeft(bullet) < 10 || Canvas.GetLeft(bullet) > 1800 || Canvas.GetTop(bullet) < 10 || Canvas.GetTop(bullet) > 980)
            {
                bulletTimer.Stop();
                form.Children.Remove(bullet); 
                bulletTimer.Tick -= MobeBulletTimerEvent;
                bullet.Source = null;
                bulletTimer = null;
            }

        }
    }
}
