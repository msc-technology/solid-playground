using PathOptimization.Extensions;

namespace PathOptimization.Validators
{
    internal class MapValueValidator
    {
        private IEnumerable<int[]> Map { get; }

        public MapValueValidator(IEnumerable<int[]> map)
        {
            Map = map;
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
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
