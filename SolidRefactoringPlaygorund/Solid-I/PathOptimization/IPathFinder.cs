namespace PathOptimization
{
    public interface IPathFinder
    {
        IEnumerable<Coordinate> Find(Coordinate start, Coordinate target, string vehicle);
    }
}
