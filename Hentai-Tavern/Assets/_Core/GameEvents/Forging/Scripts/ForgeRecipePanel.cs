using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Core._Global.CameraService;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Core._Global.ItemSystem;
using _Core._Global.Services;

namespace _Core.GameEvents.Forging.UI
{
    public sealed class ForgeRecipePanel : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private CanvasPanel panel;
        [SerializeField] private Transform recipeContainer;
        [SerializeField] private ForgeRecipeButtonView btnPrefab;
        [SerializeField] private CanvasPanel selectedPanel;
        [SerializeField] private TMP_Text   selectedName;
        [SerializeField] private Transform requirementContainer;
        [SerializeField] private ForgeRequirementView reqPrefab;
        [SerializeField] private Button startBtn;

        [Header("Setup")]
        [SerializeField] private WorkstationTag forgeTag;

        private readonly List<ForgeRecipeButtonView> btnPool   = new();
        private readonly List<ForgeRequirementView>  reqPool   = new();
        
        private IItemService items;
        private CraftRecipeConfig current;
        private UniTaskCompletionSource<CraftRecipeConfig> tcs;

        void Awake()
        {
            items = GService.GetService<IItemService>();
            startBtn.onClick.AddListener(OnStartBtn);
        }

        public void Show()
        {
            panel.Show();
            Populate();
        }

        public void Hide() => panel.Hide();

        public UniTask<CraftRecipeConfig> WaitForChoice(CancellationToken ct)
        {
            tcs = new UniTaskCompletionSource<CraftRecipeConfig>();
            using (ct.Register(() => tcs.TrySetCanceled()))
                return tcs.Task;
        }

        private void Populate()
        {
            Return(btnPool);
            var list = items.Query(s => s.Config is CraftRecipeConfig c &&
                                        s.Unlocked && c.Workstations.Contains(forgeTag))
                            .Select(s => (CraftRecipeConfig)s.Config);

            foreach (var r in list)
            {
                var v = GetBtn();
                v.transform.SetParent(recipeContainer, false);
                v.Init(r, OnClicked);
                btnPool.Add(v);
            }

            if (btnPool.Count > 0) OnClicked(btnPool[0].Recipe);
            else selectedPanel.HideInstant();
        }

        private void OnClicked(CraftRecipeConfig r)
        {
            current = r;
            selectedPanel.Show();
            selectedName.text = r.DisplayName;
            RefreshReqs();
        }

        private void RefreshReqs()
        {
            Return(reqPool);
            bool canCraft = true; int i = 0;

            foreach (var (res, cnt) in current.Materials)
            {
                var v = GetReq();
                v.transform.SetParent(requirementContainer, false);

                int have = items.GetQuantity(res);
                v.Bind(res, cnt, have);
                reqPool.Add(v);
                if (have < cnt) canCraft = false;

                v.transform.localScale = Vector3.zero;
                v.transform.DOScale(1, .25f)
                            .SetDelay(i * .03f)
                            .SetEase(Ease.OutBack);
                i++;
            }
            startBtn.interactable = canCraft;
        }

        public void OnStartBtn()
        {
            if (current == null) return;
            Hide();
            tcs.TrySetResult(current);
        }

        ForgeRecipeButtonView GetBtn() =>
            btnPool.FirstOrDefault(p => !p.gameObject.activeSelf) ??
            Instantiate(btnPrefab);

        ForgeRequirementView GetReq() =>
            reqPool.FirstOrDefault(p => !p.gameObject.activeSelf) ??
            Instantiate(reqPrefab);

        static void Return<T>(List<T> l) where T : Component
        {
            foreach (var v in l) v.gameObject.SetActive(false);
            l.Clear();
        }
    }
}
