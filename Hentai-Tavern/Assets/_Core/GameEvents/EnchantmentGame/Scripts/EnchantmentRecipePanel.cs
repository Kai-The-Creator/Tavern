using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Core._Global.ItemSystem;
using _Core._Global.Services;

namespace _Core.GameEvents.Enchantment.UI
{
    /// <summary>Панель выбора рецепта + pop-in требований.</summary>
    public sealed class EnchantmentRecipePanel : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private CanvasPanel root;

        [Header("Recipe list")]
        [SerializeField] private Transform recipeContainer;
        [SerializeField] private EnchantmentRecipeButtonView btnPrefab;

        [Header("Selected block")]
        [SerializeField] private TMP_Text    recipeName;
        [SerializeField] private Transform   reqContainer;
        [SerializeField] private RequirementView reqPrefab;
        [SerializeField] private Button      startBtn;

        [Header("Station tag")]
        [SerializeField] private WorkstationTag magicTag;

        private readonly List<EnchantmentRecipeButtonView> btnPool = new();
        private readonly List<RequirementView>             reqPool = new();
        private CraftRecipeConfig current;

        private IItemService items;

        private UniTaskCompletionSource<CraftRecipeConfig> tcs;
        private const string TweenId = "EnchantReqTween";

        public void Init(IItemService itemSvc)
        {
            items = itemSvc;
            startBtn.onClick.AddListener(OnStart);
            root.HideInstant();
        }

        public void Show()
        {
            root.Show();
            Populate();
        }

        public void Hide() => root.Hide();

        public UniTask<CraftRecipeConfig> WaitForSelection(CancellationToken ct)
        {
            tcs = new UniTaskCompletionSource<CraftRecipeConfig>();
            using (ct.Register(() => tcs.TrySetCanceled()))
                return tcs.Task;
        }

        private void Populate()
        {
            Clear(btnPool);
            var list = items.Query(s => s.Config is CraftRecipeConfig c
                                     && s.Unlocked
                                     && c.Workstations.Contains(magicTag))
                            .Select(s => (CraftRecipeConfig)s.Config)
                            .OrderBy(r => r.DisplayName);

            foreach (var r in list)
            {
                var v = GetBtn();
                v.transform.SetParent(recipeContainer, false);
                v.Init(r, OnRecipeClicked);
                btnPool.Add(v);
            }

            if (btnPool.Count > 0) OnRecipeClicked(btnPool[0].Recipe);
        }

        private void OnRecipeClicked(CraftRecipeConfig r)
        {
            current        = r;
            recipeName.text = r.DisplayName;
            DrawRequirements();
        }

        private void DrawRequirements()
        {
            DOTween.Kill(TweenId);
            Clear(reqPool);

            bool canCraft = true;
            int idx = 0;

            foreach (var (res, cnt) in current.Materials)
            {
                var v = GetReq();
                v.transform.SetParent(reqContainer, false);

                int have = items.GetQuantity(res);
                v.Bind(res, cnt, have);
                reqPool.Add(v);
                if (have < cnt) canCraft = false;

                v.transform.localScale = Vector3.zero;
                v.transform.DOScale(1, .25f)
                            .SetDelay(idx * .03f)
                            .SetEase(Ease.OutBack)
                            .SetId(TweenId);
                idx++;
            }
            startBtn.interactable = canCraft;
        }

        private void OnStart()
        {
            if (current == null) return;
            Hide();
            tcs.TrySetResult(current);
        }

        EnchantmentRecipeButtonView GetBtn() =>
            btnPool.FirstOrDefault(b => !b.gameObject.activeSelf) ?? Instantiate(btnPrefab);

        RequirementView GetReq() =>
            reqPool.FirstOrDefault(r => !r.gameObject.activeSelf) ?? Instantiate(reqPrefab);

        static void Clear<T>(List<T> l) where T : Component
        {
            foreach (var v in l) v.gameObject.SetActive(false);
            l.Clear();
        }
    }
}
