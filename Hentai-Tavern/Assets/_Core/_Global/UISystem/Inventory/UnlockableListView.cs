using System.Collections.Generic;
using _Core._Global.ItemSystem;
using _Core._Util;
using UnityEngine;

namespace _Core._Global.UI.Inventory
{
    public sealed class UnlockableListView : MonoBehaviour
    {
        [SerializeField] private UnlockableSlotView _prefab;
        [SerializeField] private Transform          _root;

        private readonly ObjectPool<UnlockableSlotView> _pool = new();

        private void Awake() => _pool.Init(_prefab, _root, 12);

        public void Show(IEnumerable<ItemState> items)
        {
            gameObject.SetActive(true);

            var list = ListPool<ItemState>.Rent();
            foreach (var s in items)
            {
                if (s.Unlocked || s.Config.ShowWhenLocked)
                    list.Add(s);
            }

            list.Sort((a, b) => string.Compare(
                a.Config.DisplayName, b.Config.DisplayName, true));

            _pool.Expand(list.Count);

            for (var i = 0; i < list.Count; i++)
            {
                var slot = _pool[i];
                slot.Bind(list[i]);
                slot.gameObject.SetActive(true);
            }

            for (var i = list.Count; i < _pool.Count; i++)
                _pool[i].gameObject.SetActive(false);

            ListPool<ItemState>.Release(list);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}