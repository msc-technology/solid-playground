using PathOptimization.PathFinders;

namespace PathOptimization.Factories
{
    public class PathFinderFactory
    {
        private readonly IDictionary<string, IWithMap<IMapValueValidator>> Constructors;

        public PathFinderFactory(IDictionary<string, IWithMap<IMapValueValidator>> constructors)
        {
            Constructors = constructors;
        }

        public PathFinder Create(string vehicle, IEnumerable<int[]> map)
        {
            bool isCtorFound = Constructors.TryGetValue(vehicle, out var validator);
            return isCtorFound
                ? new PathFinder(map, validator!.WithMap(map))
                : throw new InvalidOperationException();
        }
    }
}
