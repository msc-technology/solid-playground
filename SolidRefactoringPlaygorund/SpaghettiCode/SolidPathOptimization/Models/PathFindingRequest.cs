using PathOptimization;

namespace SolidPathOptimization.Models
{
    public class PathFindingRequest
    {
        public IEnumerable<int[]> Map { get; set; } = null!;
        public Coordinate Start { get; set; } = null!;
        public Coordinate Target { get; set; } = null!;
    }
}