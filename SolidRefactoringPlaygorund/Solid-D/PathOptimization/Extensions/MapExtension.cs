namespace PathOptimization.Extensions
{
    internal static class MapExtension
    {
        public static int GetValueAtCoordinate(this IEnumerable<int[]> map, Coordinate coordinate)
        {
            return map.ElementAt(coordinate.Y).ElementAt(coordinate.X);
        }

        public static int GetMapWidth(this IEnumerable<int[]> map)
        {
            return map.First().Count();
        }

        public static int GetMapHeight(this IEnumerable<int[]> map)
        {
            return map.Count();
        }

        public static bool IsCoordinateOutOfRange(this IEnumerable<int[]> map, Coordinate coordinate)
        {
            return coordinate.X < 0 || coordinate.Y < 0 || coordinate.Y >= map.GetMapHeight() || coordinate.X >= map.GetMapWidth();
        }
    }
}
