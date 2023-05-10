using PathOptimization.Extensions;

namespace PathOptimization.Validators
{
    public abstract class AbstractValidator : IMapValueValidator
    {
        protected IEnumerable<int[]> Map { get; set; }

        public void ValidateInputCoordinates(Coordinate start, Coordinate target)
        {
            if (start == target)
            {
                throw new ArgumentException("Start is the same as target point");
            }

            if (Map.IsCoordinateOutOfRange(start) || !IsMapValueValid(start))
            {
                throw new ArgumentException("Cannot resolve path because start coordinate is not accessibile");
            }

            if (Map.IsCoordinateOutOfRange(target) || !IsMapValueValid(target))
            {
                throw new ArgumentException("Cannot resolve path because target coordinate is not accessibile");
            }
        }

        public abstract bool IsMapValueValid(Coordinate coordinate);
    }
}
