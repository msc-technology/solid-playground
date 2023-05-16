using PathOptimization.Models;

namespace PathOptimization
{
    public interface IMapValueValidator : IPathValidator
    {
        bool IsMapValueValid(Coordinate point);
    }
}
