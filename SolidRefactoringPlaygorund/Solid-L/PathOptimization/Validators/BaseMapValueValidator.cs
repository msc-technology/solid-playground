using PathOptimization.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOptimization.Validators
{
    internal class BaseMapValueValidator
    {
        protected IEnumerable<int[]> Map { get; }

        public BaseMapValueValidator(IEnumerable<int[]> map)
        {
            Map = map;
        }

        public virtual void ValidateInputCoordinate(Coordinate start, Coordinate target)
        {
            if (start == target)
            {
                throw new ArgumentException("Start is the same as target point");
            }

            if (Map.IsCoordinateOutOfRange(start))
            {
                throw new ArgumentException("Cannot resolve path because start coordinate is out of bounds");
            }
            
            if (Map.IsCoordinateOutOfRange(target))
            {
                throw new ArgumentException("Cannot resolve path because target coordinate is out of bounds");
            }
        }
    }
}
