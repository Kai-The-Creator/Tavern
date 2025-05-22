using System.Collections.Generic;
using _Core._Global.ItemSystem;
using _Core._Util;
using UnityEngine;

namespace _Core._Global.UI.Inventory
{
    public sealed class TierListView : MonoBehaviour
    {
        [SerializeField] private TierSlotView _prefab;
        [SerializeField] private Transform    _root;

        private readonly ObjectPool<TierSlotView> _pool = new();

        private void Awake() => _pool.Init(_prefab, _root, 8);

        public void Show(IEnumerable<ItemState> items)
        {
            gameObject.SetActive(true);

            var list = ListPool<ItemState>.Rent();
            
            foreach (var st in items)
            {
                if (st.Config is not ITieredItemConfig) continue;
                list.Add(st);
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

            // скрыть остаток
            for (var i = list.Count; i < _pool.Count; i++)
                _pool[i].gameObject.SetActive(false);

            ListPool<ItemState>.Release(list);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}