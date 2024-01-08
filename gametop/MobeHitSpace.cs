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
    internal class MobeHitSpace
    {
        public double initialSphereLeft;
        public double initialSphereTop;
        public int sphereLeft;
        public int sphereTop;
        int animation = 0;

        Image player;
        public Image sphere = new Image();
        DispatcherTimer sphereTimer = new DispatcherTimer(); //179

        public List<string> downImages = new List<string> { "bub1.png", "bub2.png", "bub3.png", "bub4.png", "bub5.png", "bub6.png" };
        public int currentDownImageIndex = 0;

        public static bool hasSphereDealtDamage = false;

        public void MakeSphere(Canvas form, Image player)
        {
            this.player = player;
            sphere.Source = new BitmapImage(new Uri("bub1.png", UriKind.RelativeOrAbsolute));
            sphere.Height = 250;
            sphere.Width = 250;
            sphere.Tag = "mobesphere";
            Canvas.SetLeft(sphere, Canvas.GetLeft(player) + player.Width / 2 - sphere.Width / 2);
            Canvas.SetTop(sphere, Canvas.GetTop(player) + player.Height / 2 - sphere.Height / 2);
            Canvas.SetZIndex(sphere, 1);

            initialSphereLeft = sphereLeft;
            initialSphereTop = sphereTop;

            form.Children.Add(sphere);

            sphereTimer.Interval = TimeSpan.FromMilliseconds(20);
            sphereTimer.Tick += new EventHandler(SphereTimerEvent);
            sphereTimer.Start();

            hasSphereDealtDamage = false;
        }

        private void SphereTimerEvent(object sender, EventArgs e)
        {
            sphere.Source = new BitmapImage(new Uri(downImages[currentDownImageIndex], UriKind.RelativeOrAbsolute));
            currentDownImageIndex = (currentDownImageIndex + 1) % downImages.Count;
            animation++;

            if (animation == 6)
            {
                sphereTimer.Stop();
                sphere.Source = null;
                sphereTimer = null;
            }
        }
    }
}
