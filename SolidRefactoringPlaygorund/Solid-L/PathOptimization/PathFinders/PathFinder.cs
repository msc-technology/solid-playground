using PathOptimization.Extensions;
using PathOptimization.Validators;

namespace PathOptimization.PathFinders
{
    public class PathFinder
    {
        protected IEnumerable<int[]> Map { get; }

        public PathFinder(IEnumerable<int[]> map)
        {
            Map = map;
        }

        public IEnumerable<Coordinate> Find(Coordinate start, Coordinate target, string vehicle)
        {
            Coordinate step = new (start.X, start.Y);
            List<Coordinate> result = new() { new (step.X, step.Y) };

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

        public void ValidateInputCoordinates(Coordinate start, Coordinate target, string vehicle)
        {
            switch (vehicle)
            {
                case "vessel":
                    new VesselMapValueValidator(Map).ValidateInputCoordinate(start, target);
                    break;

                case "plane":
                    new BaseMapValueValidator(Map).ValidateInputCoordinate(start, target);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(vehicle));
            }
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

            return vehicle switch
            {
                "vessel" => nextSteps.Where(coord => Map.GetValueAtCoordinate(coord) > 0).ToArray(),
                "plane" => nextSteps.ToArray(),
                _ => throw new ArgumentOutOfRangeException(nameof(vehicle)),
            };
        }
    }
}
