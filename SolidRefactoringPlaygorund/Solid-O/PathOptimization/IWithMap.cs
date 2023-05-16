namespace PathOptimization
{
    public interface IWithMap<T> where T : class
    {
        T WithMap(IEnumerable<int[]> map);
    }
}
