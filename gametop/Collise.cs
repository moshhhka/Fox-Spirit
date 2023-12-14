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
        private UIElement player;
        private List<UIElement> elementsCopy;

        public CollisionDetector(UIElement player, List<UIElement> elementsCopy)
        {
            this.player = player;
            this.elementsCopy = elementsCopy;
        }

        public void DetectCollisions(bool goLeft, bool goRight, bool goDown, bool goUp)
        {
            foreach (UIElement u in elementsCopy)
            {
                if (u is Image image0 && (string)image0.Tag == "box") // Коллизия
                {
                    Rect rect1 = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.RenderSize.Width, player.RenderSize.Height);
                    Rect rect2 = new Rect(Canvas.GetLeft(image0), Canvas.GetTop(image0), image0.RenderSize.Width, image0.RenderSize.Height);

                    if (rect1.IntersectsWith(rect2))
                    {
                        if (goLeft)
                        {
                            goLeft = false;
                            Canvas.SetLeft(player, Canvas.GetLeft(player) + speed);
                        }

                        if (goRight)
                        {
                            goRight = false;
                            Canvas.SetLeft(player, Canvas.GetLeft(player) - speed);
                        }

                        if (goDown)
                        {
                            goDown = false;
                            Canvas.SetTop(player, Canvas.GetTop(player) - speed);
                        }

                        if (goUp)
                        {
                            goUp = false;
                            Canvas.SetTop(player, Canvas.GetTop(player) + speed);
                        }
                    }
                }
            }
        }
    }
}
