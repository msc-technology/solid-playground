using PathOptimization.Registry;

namespace PathOptimization
{
    public interface IPathFinderFactory
    {
        IPathFinder? Create(Vehicles vehicle, IEnumerable<int[]> map);
    }
}