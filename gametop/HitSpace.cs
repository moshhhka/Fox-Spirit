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
    internal class HitSpace
    {
        public double initialSphereLeft;
        public double initialSphereTop;
        public int sphereLeft;
        public int sphereTop;

        Image player;
        public Image sphere = new Image();
        DispatcherTimer sphereTimer = new DispatcherTimer();
        DispatcherTimer disappearTimer = new DispatcherTimer();

        public List<string> downImages = new List<string> { "sp4.png", "sp5.png", "sp6.png", "sp7.png", "sp8.png", "sp9.png" };
        public int currentDownImageIndex = 0;

        private bool animationCompleted = false;


        // Добавленные переменные
        private int sphereDamageCount = 0;
        private int maxSphereDamageCount = 50; // Максимальное количество урона, которое может нанести сфера

        public static bool hasSphereDealtDamage = false;

        public void MakeSphere(Canvas form, Image player)
        {
            this.player = player;
            sphere.Source = new BitmapImage(new Uri("sp4.png", UriKind.RelativeOrAbsolute));
            sphere.Height = 420;
            sphere.Width = 420;
            sphere.Tag = "sphere";
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
            if (!animationCompleted)
            {
                sphere.Source = new BitmapImage(new Uri(downImages[currentDownImageIndex], UriKind.RelativeOrAbsolute));

                // Если это первая картинка и урон еще не был нанесен, нанесите урон
                if (currentDownImageIndex == 0 && !hasSphereDealtDamage)
                {
                    // Нанесите урон здесь
                    hasSphereDealtDamage = true;
                }


                currentDownImageIndex = (currentDownImageIndex + 1) % downImages.Count;

                // Если мы прошли через все изображения, установите animationCompleted в true
                if (currentDownImageIndex == 0)
                {
                    animationCompleted = true;
                    Canvas form = (Canvas)sphere.Parent;
                    form.Children.Remove(sphere);
                }
            }
        }

    }
}
