namespace PathOptimization
{
    public interface IMapValueValidator
    {
        void ValidateInputCoordinates(Coordinate start, Coordinate target);
        bool IsMapValueValid(Coordinate coordinate);
    }
}
