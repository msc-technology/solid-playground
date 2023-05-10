using PathOptimization.Extensions;
using System.Diagnostics;

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

        public void ValidateInputCoordinates(Coordinate start, Coordinate target, string vehicle)
        {
            if (start == target)
            {
                throw new ArgumentException("Start is the same as target point");
            }

            switch (vehicle)
            {
                case "vessel":
                    switch (Map.IsCoordinateOutOfRange(start) || Map.GetValueAtCoordinate(start) <= 0)
                    {
                        case true:
                            throw new ArgumentException("Cannot resolve path because start coordinate is not accessibile");
                        default:
                            break;
                    }
                    switch (Map.IsCoordinateOutOfRange(target) || Map.GetValueAtCoordinate(target) <= 0)
                    {
                        case true:
                            throw new ArgumentException("Cannot resolve path because target coordinate is not accessibile");
                        default:
                            break;
                    }
                    break;

                case "plane":
                    switch (Map.IsCoordinateOutOfRange(start))
                    {
                        case true:
                            throw new ArgumentException("Cannot resolve path because start coordinate is not accessibile");
                        default:
                            break;
                    }
                    switch (Map.IsCoordinateOutOfRange(target))
                    {
                        case true:
                            throw new ArgumentException("Cannot resolve path because target coordinate is not accessibile");
                        default:
                            break;
                    }
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

            switch (vehicle)
            {
                case "vessel":
                    return nextSteps.Where(coord => Map.GetValueAtCoordinate(coord) > 0).ToArray();
                case "plane":
                    return nextSteps.ToArray();
                default: throw new ArgumentOutOfRangeException(nameof(vehicle));
            }
        }
    }
}
