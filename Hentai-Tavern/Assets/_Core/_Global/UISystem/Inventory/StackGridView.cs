using System.Collections.Generic;
using _Core._Global.ItemSystem;
using UnityEngine;
using UnityEngine.UI;
using _Core._Util;

namespace _Core._Global.UI.Inventory
{
    public sealed class StackGridView : MonoBehaviour
    {
        [Header("Prefabs & Layout")]
        [SerializeField] private StackCellView _cellPrefab;
        [SerializeField] private StackItemView _itemPrefab;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private int _initialCells = 35;

        private readonly ObjectPool<StackCellView> _cellPool  = new();
        private readonly ObjectPool<StackItemView> _itemPool  = new();

        private void Awake()
        {
            _cellPool.Init(_cellPrefab, _grid.transform, _initialCells);
            _itemPool.Init(_itemPrefab, _grid.transform, 0);
        }

        public void Show(IEnumerable<ItemState> items)
        {
            gameObject.SetActive(true);

            // --- подготовка списка предметов >0 ---
            var list = ListPool<ItemState>.Rent();
            foreach (var s in items)
                if (s.Quantity > 0) list.Add(s);

            // --- гарантируем нужное число ячеек ---
            _cellPool.Expand(Mathf.Max(_initialCells, list.Count));

            // --- расставляем предметы ---
            int itemIdx = 0;
            for (int i = 0; i < _cellPool.Count; i++)
            {
                var cell = _cellPool[i];

                // если в ячейке уже что-то лежало – убираем
                if (cell.HasItem)
                {
                    _itemPool.Release(cell.Item);
                    cell.Detach();
                }

                bool needItem = itemIdx < list.Count;
                cell.gameObject.SetActive(true);

                if (needItem)
                {
                    var view = _itemPool.Get();
                    view.Bind(list[itemIdx++], cell);
                }
            }

            // лишние ItemView назад в пул
            while (_itemPool.ActiveCount > list.Count)
                _itemPool.Release(_itemPool[_itemPool.ActiveCount - 1]);

            ListPool<ItemState>.Release(list);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
