namespace _Core._Global
{
    using System.Collections.Generic;

    public static class ListPool<T>
    {
        private static readonly Stack<List<T>> _cache = new();

        public static List<T> Rent()
        {
            return _cache.Count > 0 ? _cache.Pop() : new List<T>();
        }

        public static void Release(List<T> list)
        {
            list.Clear();
            _cache.Push(list);
        }
    }
}