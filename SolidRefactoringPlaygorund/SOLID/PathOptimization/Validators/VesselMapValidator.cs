using PathOptimization.Extensions;
using PathOptimization.Models;

namespace PathOptimization.Validators
{
    public class VesselMapValidator : IMapValueValidator, IWithMap<IMapValueValidator>
    {
        protected IEnumerable<int[]> Map { get; set; } = null!;
        private IWithMap<IPathValidator> PathValidator { get; }

        public VesselMapValidator(IWithMap<IPathValidator> pathValidator)
        {
            PathValidator = pathValidator;
        }

        public bool IsMapValueValid(Coordinate point)
        {
            return point != null && Map.GetValueAtCoordinate(point) > 0;
        }

        public IMapValueValidator WithMap(IEnumerable<int[]> map)
        {
            Map = map;
            return this;
        }

        public void ValidatePathCoordinates(Coordinate start, Coordinate target)
        {
            PathValidator.WithMap(Map).ValidatePathCoordinates(start, target); 
            
            if (!IsMapValueValid(start))
            {
                throw new ArgumentException("Cannot resolve path because start coordinate is not accessibile");
            }

            if (!IsMapValueValid(target))
            {
                throw new ArgumentException("Cannot resolve path because target coordinate is not accessibile");
            }
        }
    }
}
