using PathOptimization.Extensions;
using PathOptimization.Models;

namespace PathOptimization.Validators
{
    public class PathValidator : IPathValidator, IWithMap<IPathValidator>
    {
        protected IEnumerable<int[]> Map { get; set; }

        public void ValidatePathCoordinates(Coordinate start, Coordinate target)
        {
            if (start == target)
            {
                throw new ArgumentException("Start is the same as target point");
            }

            if (Map.IsCoordinateOutOfRange(start))
            {
                throw new ArgumentException("Cannot resolve path because start coordinate is not accessibile");
            }

            if (Map.IsCoordinateOutOfRange(target))
            {
                throw new ArgumentException("Cannot resolve path because target coordinate is not accessibile");
            }
        }

        public IPathValidator WithMap(IEnumerable<int[]> map)
        {
            Map = map;
            return this;
        }
    }
}
