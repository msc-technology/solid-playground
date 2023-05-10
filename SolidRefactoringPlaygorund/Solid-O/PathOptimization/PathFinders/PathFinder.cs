using PathOptimization.Extensions;

namespace PathOptimization.PathFinders
{
    public class PathFinder
    {
        protected IEnumerable<int[]> Map { get; }
        private IMapValueValidator Validator { get; }

        public PathFinder(IEnumerable<int[]> map, IMapValueValidator validator)
        {
            Map = map;
            Validator = validator;
        }

        public IEnumerable<Coordinate> Find(Coordinate start, Coordinate target)
        {
            Coordinate step = new (start.X, start.Y);
            List<Coordinate> result = new() { new (step.X, step.Y) };

            while (step != target)
            {
                IEnumerable<Coordinate> availableSteps = GetAvailableSteps(step)
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

        public void ValidateInputCoordinates(Coordinate start, Coordinate target)
        {
            Validator.ValidateInputCoordinates(start, target);
        }

        private IEnumerable<Coordinate> GetAvailableSteps(Coordinate point)
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

            return nextSteps.Where(coord => Validator.IsMapValueValid(coord)).ToArray();
        }
    }
}
