using System.Collections.Generic;
using _Core._Global.ItemSystem;
using _Core._Global.Services;
using _Core._Global.UISystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UI.Inventory
{
    public sealed class InventoryWindow : UIWindow
    {
        [Header("UI Parts")]
        [SerializeField] private ToggleGroup          _tabGroup;
        [SerializeField] private StackGridView        _stackView;
        [SerializeField] private TierListView         _tierView;
        [SerializeField] private UnlockableListView   _unlockView;
        [SerializeField] private RectTransform _dragLayer;

        public RectTransform DragLayer => _dragLayer;

        private InventoryPresenter _presenter;

        public override async UniTask Show(WindowLayer layer)
        {
            await base.Show(layer);
            if (_presenter == null)
            {
                _presenter = new InventoryPresenter(this, GService.GetService<IItemService>());
                _presenter.Init(_tabGroup);
            }
            _presenter.ShowFirstTab(_tabGroup);
        }


        // —————————————————————————— called from Presenter
        public void ShowStack(IEnumerable<ItemState> items)
        {
            _stackView.Show(items);
            _tierView.Hide();
            _unlockView.Hide();
        }

        public void ShowTier(IEnumerable<ItemState> items)
        {
            _tierView.Show(items);
            _stackView.Hide();
            _unlockView.Hide();
        }

        public void ShowUnlockable(IEnumerable<ItemState> items)
        {
            _unlockView.Show(items);
            _stackView.Hide();
            _tierView.Hide();
        }
    }
}