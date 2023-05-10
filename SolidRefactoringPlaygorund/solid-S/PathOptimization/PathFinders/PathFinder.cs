using PathOptimization.Extensions;
using PathOptimization.Validators;

namespace PathOptimization.PathFinders
{
    public class PathFinder
    {
        protected IEnumerable<int[]> Map { get; }
        private MapValueValidator Validator { get; }

        public PathFinder(IEnumerable<int[]> map)
        {
            Map = map;
            Validator = new MapValueValidator(map);
        }

        public IEnumerable<Coordinate> Find(Coordinate start, Coordinate target, string vehicle)
        {
            Validator.ValidateInputCoordinates(start, target, vehicle);

            Coordinate step = new Coordinate(start.X, start.Y);
            List<Coordinate> result = new() { new Coordinate(step.X, step.Y) };

            while (step != target)
            {
                IEnumerable<Coordinate> availableSteps = GetAvailableSteps(step, vehicle)
                    .Where(x => !result.Contains(x))
                    .ToList();
                if (!availableSteps.Any())
                {
                    throw new InvalidOperationException("Cannot resolve path");
                }

                int minDistance = availableSteps.Select(y => y.GetDistance(target)).Min();

                IEnumerable<Coordinate> convenientSteps = availableSteps.Where(coord => coord.GetDistance(target) == minDistance).ToArray();
                int maxValueAtConvenientSteps = convenientSteps.Select(x => Map.GetValueAtCoordinate(x)).Max();

                step = convenientSteps.Count() == 1 ?
                    convenientSteps.First() :
                    convenientSteps.First(coord => Map.GetValueAtCoordinate(coord) == maxValueAtConvenientSteps);

                result.Add(new Coordinate(step.X, step.Y));
            }

            return result;
        }

        private IEnumerable<Coordinate> GetAvailableSteps(Coordinate point, string vehicle)
        {
            List<Coordinate> nextSteps = new();
            if (point.Y - 1 >= 0)
            {
                nextSteps.Add(new Coordinate(point.X, point.Y - 1));
            }
            if (point.Y + 1 < Map.GetMapHeight())
            {
                nextSteps.Add(new Coordinate(point.X, point.Y + 1));
            }
            if (point.X - 1 >= 0)
            {
                nextSteps.Add(new Coordinate(point.X - 1, point.Y));
            }
            if (point.X + 1 < Map.GetMapWidth())
            {
                nextSteps.Add(new Coordinate(point.X + 1, point.Y));
            }

            switch (vehicle)
            {
                case "vessel":
                    return nextSteps.Where(coord => Map.GetValueAtCoordinate(coord) > 0).ToArray();
                case "plane":
                    return nextSteps.ToArray();
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
