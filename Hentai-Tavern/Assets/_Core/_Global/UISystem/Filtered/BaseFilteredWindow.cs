using System;
using System.Collections.Generic;
using System.Linq;
using _Core._Global.Categories;
using _Core._Global.Categories.Filtering;
using _Core._Global.UISystem.ItemsView;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace _Core._Global.UISystem.Filtered
{
    public abstract class BaseFilteredWindow<TItem, TContext, TView> : UIWindow where TView : BaseItemView<TContext>
    {
        [Header("Filter UI")] [SerializeField] protected CategoryTreeView categoryTreeView;
        [SerializeField] protected TMP_InputField searchInput;
        [SerializeField] protected SortDropdownView sortDropdown;

        [Header("Items Container & Prefab")] [SerializeField]
        protected Transform itemsContainer;

        [SerializeField] protected TView itemPrefab;

        protected FilterPresenter<TItem> _presenter;
        private ObjectPool<TView> _pool;
        protected readonly Dictionary<TItem, TView> _activeViews = new();


        protected override void Awake()
        {
            InitializeServices();
            base.Awake();
        }

        public override async UniTask Show(WindowLayer layer)
        {
            InitializeFiltering();
            RebuildUI();
            await base.Show(layer);
        }

        public override async UniTask Close()
        {
            TearDownFiltering();
            await base.Close();
        }

        protected abstract void InitializeServices();

        private void InitializeFiltering()
        {
            _pool = new ObjectPool<TView>(itemPrefab, itemsContainer, initialSize: 10);

            var typeFilter = new DelegateFilter<TItem>(FilterPredicate);
            var searchFilter = new SearchFilter<TItem>(GetSearchText);
            var categoryFilter = new CategoryFilter<TItem>(GetCategories);

            var group = new FilterGroup<TItem>
            {
                GroupMode = FilterGroup<TItem>.Mode.And
            };
            group.Filters.Add(typeFilter);
            group.Filters.Add(searchFilter);
            group.Filters.Add(categoryFilter);

            var sorter = new Sorter<TItem, object>
            {
                KeySelector = GetSortKey,
                Ascending = true
            };

            _presenter = new FilterPresenter<TItem>(group, sorter);
            _presenter.OnCriteriaChanged += RebuildUI;

            categoryTreeView.OnNodeToggled += _presenter.SetCategoryNode;
            categoryTreeView.Build(GetCategoryRoots());
            categoryTreeView.SetAvailableCategories(
                GetAllItems().SelectMany(GetCategories).Distinct()
            );

            searchInput.onValueChanged.AddListener(_presenter.SetSearchTerm);

            sortDropdown.Build(new List<(string, Action)>
            {
                ("A→Z", () => _presenter.SetSort(GetSortKey, true)),
                ("Z→A", () => _presenter.SetSort(GetSortKey, false)),
            });

            SubscribeToDataEvents();
        }


        private void TearDownFiltering()
        {
            _presenter.OnCriteriaChanged -= RebuildUI;
            categoryTreeView.OnNodeToggled -= _presenter.SetCategoryNode;
            searchInput.onValueChanged.RemoveListener(_presenter.SetSearchTerm);

            UnsubscribeFromDataEvents();
            _pool.ReleaseAll(_activeViews.Values);
            _activeViews.Clear();
        }

        protected void RebuildUI()
        {
            var allItems = GetAllItems().ToList();

            var allCats = allItems
                .SelectMany(GetCategories)
                .Distinct();
            categoryTreeView.SetAvailableCategories(allCats);

            var filtered = _presenter.Apply(allItems).ToList();
            var filteredSet = new HashSet<TItem>(filtered);

            foreach (var kv in _activeViews.ToList())
            {
                if (!filteredSet.Contains(kv.Key))
                {
                    _pool.Release(kv.Value);
                    _activeViews.Remove(kv.Key);
                }
            }

            for (int i = 0; i < filtered.Count; i++)
            {
                var item = filtered[i];
                if (!_activeViews.TryGetValue(item, out var view))
                {
                    view = _pool.Get();
                    var ctx = CreateContext(item);
                    view.OnSpawn(ctx);
                    _activeViews[item] = view;
                }

                view.transform.SetSiblingIndex(i);
                view.AnimateIn();
            }
        }

        protected abstract TContext CreateContext(TItem item);

        protected abstract IEnumerable<TItem> GetAllItems();
        protected abstract IEnumerable<CategoryDefinition> GetCategories(TItem item);
        protected abstract string GetSearchText(TItem item);
        protected abstract object GetSortKey(TItem item);
        protected abstract List<CategoryNode> GetCategoryRoots();
        protected abstract void SubscribeToDataEvents();
        protected abstract void UnsubscribeFromDataEvents();
        protected abstract bool FilterPredicate(TItem item);

        private class DelegateFilter<U> : IFilter<U>
        {
            private readonly Func<U, bool> _pred;
            public DelegateFilter(Func<U, bool> pred) => _pred = pred;
            public bool Match(U item) => _pred(item);
        }
    }
}