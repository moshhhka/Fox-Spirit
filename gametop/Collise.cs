using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace gametop
{
    class CollisionDetector
    {
        private int speed = 20;
        public Image player;
        public List<UIElement> elementsCopy;

        public CollisionDetector(Image player, List<UIElement> elementsCopy, int speed = 20)
        {
            this.player = player;
            this.elementsCopy = elementsCopy;
            this.speed = speed;
        }

        public void DetectCollisions()
        {
            foreach (UIElement u in elementsCopy)
            {
                if (u is Image image0 && (string)image0.Tag == "box") // Коллизия
                {
                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.RenderSize.Width, player.RenderSize.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image0), Canvas.GetTop(image0), image0.RenderSize.Width, image0.RenderSize.Height);

                    if (rect1.IntersectsWith(rect2))
                    {
                        if (Player.goLeft)
                        {
                            Player.goLeft = false;
                            Canvas.SetLeft(player, Canvas.GetLeft(player) + speed);
                        }

                        if (Player.goRight)
                        {
                            Player.goRight = false;
                            Canvas.SetLeft(player, Canvas.GetLeft(player) - speed);
                        }

                        if (Player.goDown)
                        {
                            Player.goDown = false;
                            Canvas.SetTop(player, Canvas.GetTop(player) - speed);
                        }

                        if (Player.goUp)
                        {
                            Player.goUp = false;
                            Canvas.SetTop(player, Canvas.GetTop(player) + speed);
                        }
                    }
                }
            }
        }
    }
}
