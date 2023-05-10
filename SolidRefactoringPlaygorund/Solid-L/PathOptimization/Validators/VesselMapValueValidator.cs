using PathOptimization.Extensions;

namespace PathOptimization.Validators
{
    internal class VesselMapValueValidator : BaseMapValueValidator
    {
        public VesselMapValueValidator(IEnumerable<int[]> map) : base(map)
        {
        }

        public override void ValidateInputCoordinate(Coordinate start, Coordinate target)
        {
            base.ValidateInputCoordinate(start, target);

            if (Map.GetValueAtCoordinate(start) <= 0)
            {
                throw new ArgumentException("Cannot resolve path because start coordinate is not accessibile");
            }

            if (Map.GetValueAtCoordinate(target) <= 0)
            {
                throw new ArgumentException("Cannot resolve path because target coordinate is not accessibile");
            }
        }

        public override bool IsStepValid(Coordinate point)
        {
            return Map.GetValueAtCoordinate(point) > 0;
        }
    }
}
