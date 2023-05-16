namespace PathOptimization.Validators
{
    public class PlaneMapValueValidator : AbstractValidator, IMapValueValidator, IWithMap<IMapValueValidator>
    {
        public override bool IsMapValueValid(Coordinate coordinate)
        {
            return true;
        }

        public IMapValueValidator WithMap(IEnumerable<int[]> map)
        {
            Map = map;
            return this;
        }
    }
}