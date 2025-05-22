namespace _Core._Global.Categories.Filtering
{
    public interface IFilter<T>
    {
        bool Match(T item);
    }
}