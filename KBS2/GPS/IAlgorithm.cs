using KBS2.CarSystem;

namespace KBS2.GPS
{
    public interface IAlgorithm
    {
        Destination Calculate(Destination carDestination, Destination endDestination);
    }
}