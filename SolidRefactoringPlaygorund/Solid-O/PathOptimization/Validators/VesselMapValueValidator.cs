using PathOptimization.Extensions;

namespace PathOptimization.Validators
{
    public class VesselMapValueValidator : AbstractValidator, IMapValueValidator, IWithMap<IMapValueValidator>
    {
        public override bool IsMapValueValid(Coordinate coordinate)
        {
            return Map.GetValueAtCoordinate(coordinate) > 0;
        }

        public IMapValueValidator WithMap(IEnumerable<int[]> map)
        {
            Map = map;
            return this;
        }
    }
}