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

            MainWindow.Loop.Subscribe(this.Update);
        }

        public void Update()
        {
            Canvas.SetTop(Sprite, Location.Y - Size.Y);
            Canvas.SetLeft(Sprite, Location.X - Size.X);
        }

        public void Destroy()
        {
            Canvas.Children.Remove(Sprite);

            MainWindow.Loop.Unsubscribe(this.Update);
        }

        public void ChangeSprite(BitmapImage image)
        {
            Sprite.Source = image;
            Size = new Vector((int)(Sprite.Width / 2), (int)(Sprite.Height / 2));
        }

        public void Rotate(double angle)
        {
            Sprite.RenderTransform = new RotateTransform(angle, Size.X, Size.Y);
        }
    }
}
