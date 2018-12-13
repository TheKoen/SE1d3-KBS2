using KBS2.CarSystem;

namespace Algorithms
{
    public interface IAlgorithm
    {
        Destination Calculate(Destination carDestination, Destination endDestination);
    }
}