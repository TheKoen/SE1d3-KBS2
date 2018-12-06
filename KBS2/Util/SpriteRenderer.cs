using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KBS2.Util
{
    class SpriteRenderer
    {
        private Image Sprite { get; }
        private Vector Size { get; set; }
        private Canvas Canvas { get; }
        private Vector Location { get; set; }

        /// <summary>
        /// Create a spriterender for a GameObject
        /// </summary>
        /// <param name="sprite">image</param>
        /// <param name="location">location</param>
        /// <param name="canvas">canvas where it needs to be drawn on</param>
        public SpriteRenderer(Image sprite, Vector location, Canvas canvas)
        {
            Canvas = canvas;
            Location = location;
            Sprite = new Image()
            {
                Source = sprite.Source
            };
            Size = new Vector((int)(sprite.Width / 2), (int)(sprite.Height / 2));

            canvas.Children.Add(Sprite);

            MainScreen.Loop.Subscribe(this.Update);
        }

        public void Update()
        {
            Canvas.SetTop(Sprite, Location.Y - Size.Y);
            Canvas.SetLeft(Sprite, Location.X - Size.X);
        }

        /// <summary>
        /// Remove the sprite from canvas en unsubscribe from loop
        /// </summary>
        public void Destroy()
        {
            Canvas.Children.Remove(Sprite);

            MainScreen.Loop.Unsubscribe(this.Update);
        }

        /// <summary>
        /// change the sprite
        /// </summary>
        /// <param name="image">image</param>
        public void ChangeSprite(BitmapImage image)
        {
            Sprite.Source = image;
            Size = new Vector((int)(Sprite.Width / 2), (int)(Sprite.Height / 2));
        }

        /// <summary>
        /// Rotate the sprite
        /// </summary>
        /// <param name="angle">double angle</param>
        public void Rotate(double angle)
        {
            Sprite.RenderTransform = new RotateTransform(angle, Size.X, Size.Y);
        }
    }
}
