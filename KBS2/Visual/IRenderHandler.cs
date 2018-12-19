using System.Windows.Controls;

namespace KBS2.Visual
{
    public interface IRenderHandler
    {
        Canvas _Canvas { get; }

        void Update();
    }
}
