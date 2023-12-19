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
    internal class Sword
    {
        public double initialSwordLeft;
        public double initialSwordTop;
        public string direction;
        public int swordLeft;
        public int swordTop;

        private int speed = 5;
        private Image sword = new Image();
        DispatcherTimer swordTimer = new DispatcherTimer();



        public void MakeSword(Canvas form)
        {
            sword.Source = new BitmapImage(new Uri("sword.png", UriKind.RelativeOrAbsolute));
            sword.Height = 167;
            sword.Width = 160;
            sword.Tag = "sword";
            Canvas.SetLeft(sword, swordLeft);
            Canvas.SetTop(sword, swordTop);
            Canvas.SetZIndex(sword, 1);

            initialSwordLeft = swordLeft;
            initialSwordTop = swordTop;

            form.Children.Add(sword);


            swordTimer.Interval = TimeSpan.FromMilliseconds(speed);
            swordTimer.Tick += new EventHandler(SwordTimerEvent);
            swordTimer.Start();


        }

        private void SwordTimerEvent(object sender, EventArgs e)
        {
            if (direction == "left")
            {
                Canvas.SetLeft(sword, Canvas.GetLeft(sword) - speed);
            }

            if (direction == "right")
            {
                Canvas.SetLeft(sword, Canvas.GetLeft(sword) + speed);
                sword.Source = new BitmapImage(new Uri("swordr.png", UriKind.RelativeOrAbsolute));
            }

            if (direction == "up")
            {
                Canvas.SetTop(sword, Canvas.GetTop(sword) - speed);
            }

            if (direction == "down")
            {
                Canvas.SetTop(sword, Canvas.GetTop(sword) + speed);
            }


            if (Math.Abs(Canvas.GetLeft(sword) - initialSwordLeft) > 150 || Math.Abs(Canvas.GetTop(sword) - initialSwordTop) > 150)
            {
                swordTimer.Stop();
                swordTimer.Stop();
                sword.Source = null;
                swordTimer = null;

            }
        }
    }
}
