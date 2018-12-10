using System.Windows.Controls;

namespace KBS2.Visual
{
    public interface IRenderHandler
    {
        Canvas Canvas { get; }

        void Update();
    }
}
