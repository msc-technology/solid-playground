using PathOptimization.PathFinders;

namespace PathOptimization.Factories
{
    public class PathFinderFactory
    {
        public static PathFinder? Create(IEnumerable<int[]> map)
        {
            return map is null ? 
                null :
                new PathFinder(map);
        }
    }
}
