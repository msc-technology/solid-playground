using PathOptimization.Models;

namespace PathOptimization
{
    public interface IPathValidator
    {
        void ValidatePathCoordinates(Coordinate start, Coordinate target);
    }
}