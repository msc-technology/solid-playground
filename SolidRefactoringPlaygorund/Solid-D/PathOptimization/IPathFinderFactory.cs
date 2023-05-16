using PathOptimization.PathFinders;

namespace PathOptimization
{
    public interface IPathFinderFactory
    {
        public PathFinder? Create(IEnumerable<int[]> map);
    }
}
