using KBS2.CarSystem;

namespace KBS2.GPS
{
    public interface IAlgorithm
    {
        Destination Calculate(long callerId, Destination carDestination, Destination endDestination);
    }
}