using PathOptimization.Extensions;
using PathOptimization.Models;

namespace PathOptimization.Validators
{
    public class PlaneMapValidator : IMapValueValidator, IWithMap<IMapValueValidator>
    {
        private IEnumerable<int[]> Map { get; set; } = null!;
        private IWithMap<IPathValidator> PathValidator { get; }

        public PlaneMapValidator(IWithMap<IPathValidator> pathValidator)
        {
            PathValidator = pathValidator;
        }

        public bool IsMapValueValid(Coordinate point)
        {
            return point is not null && Map.GetValueAtCoordinate(point) >= 0;
        }

        public IMapValueValidator WithMap(IEnumerable<int[]> map)
        {
            Map = map;
            return this;
        }

        public void ValidatePathCoordinates(Coordinate start, Coordinate target)
        {
            PathValidator.WithMap(Map).ValidatePathCoordinates(start, target);
        }
    }
}
