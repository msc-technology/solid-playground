using PathOptimization.PathFinders;
using PathOptimization.Registry;

namespace PathOptimization.Factories
{
    public class PathFinderFactory : IPathFinderFactory
    {
        private readonly IDictionary<Vehicles, IWithMap<IMapValueValidator>> Constructors;

        public PathFinderFactory(IDictionary<Vehicles, IWithMap<IMapValueValidator>> constructors)
        {
            Constructors = constructors;
        }

        public IPathFinder? Create(Vehicles vehicle, IEnumerable<int[]> map)
        {
            if (map is null)
            {
                return null;
            }
            else
            {
                bool isCtorFound = Constructors.TryGetValue(vehicle, out var validator);
                return isCtorFound
                    ? new PathFinder(map, validator!.WithMap(map))
                    : null;
            }
        }
    }
}
