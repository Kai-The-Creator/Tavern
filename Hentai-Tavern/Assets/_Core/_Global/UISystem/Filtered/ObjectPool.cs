using System.Collections.Generic;
using _Core._Global.UISystem.ItemsView;
using UnityEngine;

namespace _Core._Global.UISystem.Filtered
{
    public interface IPoolable
    {
        void OnSpawn(object context) { }
        void OnDespawn() { }
    }

    public class ObjectPool<T> where T : MonoBehaviour, IPoolable
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Stack<T> _pool = new Stack<T>();

        public ObjectPool(T prefab, Transform parent, int initialSize = 10)
        {
            _prefab = prefab;
            _parent = parent;
            for (int i = 0; i < initialSize; i++)
                _pool.Push(CreateNew());
        }

        private T CreateNew()
        {
            var inst = Object.Instantiate(_prefab, _parent);
            inst.gameObject.SetActive(false);
            return inst;
        }

        public T Get()
        {
            var item = _pool.Count > 0 ? _pool.Pop() : CreateNew();
            item.gameObject.SetActive(true);
            return item;
        }

        public void Release(T item)
        {
            item.OnDespawn();
            item.gameObject.SetActive(false);
            _pool.Push(item);
        }

        public void ReleaseAll(IEnumerable<T> activeItems)
        {
            foreach (var item in activeItems)
                Release(item);
        }
    }
}