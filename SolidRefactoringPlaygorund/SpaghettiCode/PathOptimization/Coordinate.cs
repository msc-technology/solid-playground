namespace PathOptimization
{
    public record class Coordinate
    {
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public int GetDistance(Coordinate target)
        {
            return Math.Abs(X - target.X) + Math.Abs(Y - target.Y);
        }
    }
}