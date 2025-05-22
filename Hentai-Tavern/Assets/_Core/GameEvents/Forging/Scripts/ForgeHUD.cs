using System;
using _Core._Global.CameraService;
using _Core._Global.Services;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.GameEvents.Forging.UI
{
    public sealed class ForgeHUD : MonoBehaviour
    {
        [Header("Canvas")]
        [SerializeField] private Canvas canvas;

        [Header("Game panel")]
        [SerializeField] private CanvasPanel panelGame;
        [SerializeField] private Slider      bar;
        [SerializeField] private TMP_Text    lives;

        [Header("Result panel")]
        [SerializeField] private CanvasPanel panelResult;
        [SerializeField] private TMP_Text    resultText;
        [SerializeField] private Button      btnCraftMore;
        [SerializeField] private Button      btnFinish;

        void Awake() => canvas.worldCamera =
            GService.GetService<ICameraService>().GetUICamera();

        #region Game HUD --------------------------------------------------
        public void ShowGame()
        {
            panelGame.Show();
            panelResult.HideInstant();
            bar.value = 0;
        }

        public void HideAll()
        {
            panelGame.HideInstant();
            panelResult.HideInstant();
        }

        public void UpdateProgress(float n01)          => bar.value = n01;
        public void UpdateLives(int v, bool inf)       => lives.text = inf ? "∞" : v.ToString();

        public UniTask TweenProgress(float n01, float d) =>
            bar.DOValue(n01, d).SetEase(Ease.OutQuad).ToUniTask();
        #endregion

        #region Result panel ---------------------------------------------
        /// <summary>
        /// Показывает результат и вешает колбэки на две кнопки.
        /// </summary>
        public void ShowResult(string msg, Action onMore, Action onFinish)
        {
            panelGame.Hide();
            resultText.text = msg;

            btnCraftMore.onClick.RemoveAllListeners();
            btnFinish   .onClick.RemoveAllListeners();

            btnCraftMore.onClick.AddListener(() => onMore?.Invoke());
            btnFinish   .onClick.AddListener(() => onFinish?.Invoke());

            panelResult.Show();
        }
        #endregion
    }
}
