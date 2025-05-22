using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Core._Global.CameraService;
using _Core._Global.ItemSystem;
using _Core._Global.Services;
using _Core.GameEvents.Enchantment.Data;

namespace _Core.GameEvents.Enchantment.UI
{
    public sealed class EnchantmentHUD : MonoBehaviour
    {
        private const string TweenId = "EnchantHUDTween";

        [Header("Canvas")]
        [SerializeField] private Canvas canvas;

        [Header("Recipe panel")]
        [SerializeField] private EnchantmentRecipePanel recipePanel;

        [Header("Game panel")]
        [SerializeField] private CanvasPanel panelGame;
        [SerializeField] private Slider      progressBar;
        [SerializeField] private TMP_Text    livesTxt;

        [Header("Result panel")]
        [SerializeField] private CanvasPanel panelResult;
        [SerializeField] private TMP_Text    resultTxt;
        [SerializeField] private Button      btnMore;
        [SerializeField] private Button      btnFinish;

        [Header("Rune buttons (Inspector order == RunePool order)")]
        [SerializeField] private RuneView[] runeViews;

        private void Awake() =>
            canvas.worldCamera = GService.GetService<ICameraService>().GetUICamera();

        public void Init(IItemService items, WorkstationTag tag, RuneData[] pool)
        {
            recipePanel.Init(items);
            for (int i = 0; i < runeViews.Length && i < pool.Length; i++)
                runeViews[i].SetData(pool[i]);

            ClearRunes();
        }

        public void ShowRecipe() => recipePanel.Show();
        public UniTask<CraftRecipeConfig> WaitForRecipe(CancellationToken ct) =>
            recipePanel.WaitForSelection(ct);
        public void HideRecipe() => recipePanel.Hide();

        public void ShowGame(int goalRounds, int lives, bool infLives)
        {
            panelGame.Show();
            panelResult.HideInstant();

            progressBar.minValue = 0;
            progressBar.maxValue = goalRounds;
            progressBar.value    = 0;

            UpdateLives(lives, infLives);
        }

        public void UpdateRound(int finished)  => progressBar.value = finished;

        public UniTask TweenRound(int finished,float dur)=>
            progressBar.DOValue(finished, dur)
                       .SetEase(Ease.OutQuad)
                       .SetId(TweenId)
                       .ToUniTask();

        public void UpdateLives(int v,bool inf)=>
            livesTxt.text = inf ? "∞" : v.ToString();

        public void ShowResult(string msg, Action onMore, Action onFinish)
        {
            panelGame.Hide();
            resultTxt.text = msg;

            btnMore .onClick.RemoveAllListeners();
            btnFinish.onClick.RemoveAllListeners();

            if (onMore  != null) btnMore .onClick.AddListener(() => onMore.Invoke());
            if (onFinish!= null) btnFinish.onClick.AddListener(() => onFinish.Invoke());

            panelResult.Show();
        }

        public RuneView GetRune(int idx) =>
            idx >= 0 && idx < runeViews.Length ? runeViews[idx] : null;

        /// <summary>Заполняет RuneView и назначает клик-callback. Ввод должен быть разблокирован.</summary>
        public void BindRune(int idx, RuneData data, Action<RuneView> cb)
        {
            var rv = GetRune(idx);
            if (rv == null) return;
            rv.SetData(data);
            rv.SetCallback(cb);
        }

        public void ClearRunes()
        {
            foreach (var rv in runeViews)
            {
                rv.SetCallback(null);
                rv.SetInteractable(false);
            }
        }

        /// <summary>Блок/разблок всех RuneView (hover/click).</summary>
        public void LockRunes(bool lockInput)
        {
            foreach (var rv in runeViews)
                rv.SetInteractable(!lockInput);
        }

        public async UniTask FlashSequence(IReadOnlyList<RuneView> seq,
                                           float show, float pause,
                                           CancellationToken ct)
        {
            foreach (var rv in seq)
            {
                await rv.Flash(show);
                await UniTask.Delay(Mathf.RoundToInt(pause*1000), cancellationToken:ct);
            }
        }

        public void HideAll()
        {
            recipePanel.Hide();
            panelGame.HideInstant();
            panelResult.HideInstant();
        }

        public void PauseTweens()  => DOTween.Pause(TweenId);
        public void ResumeTweens() => DOTween.Play (TweenId);
    }
}
