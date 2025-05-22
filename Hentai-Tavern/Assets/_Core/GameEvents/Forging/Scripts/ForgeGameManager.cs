using System.Threading;
using _Core._Global.CameraService;
using _Core._Global.ItemSystem;
using _Core._Global.Services;
using _Core.GameEvents.Common;
using _Core.GameEvents.Forging.Data;
using _Core.GameEvents.Forging.States;
using _Core.GameEvents.Forging.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Core.GameEvents.Forging.Scripts
{
    public sealed class ForgeGameManager : MonoBehaviour, IMiniGameManager
    {
        [Header("Config")]
        [SerializeField] private ForgeGameConfig config;

        [Header("UI")]
        [SerializeField] private ForgeRecipePanel  recipePanel;
        [SerializeField] private ForgeTargetSpawner targetSpawner;
        [SerializeField] private ForgeHUD           hud;

        [Header("Camera pivots")]
        [SerializeField] private Transform camStart;
        [SerializeField] private Transform camFinal;

        [Header("Spawn & Final")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform productPivot;

        internal CraftRecipeConfig CurrentRecipe { get; set; }
        internal ProductModel CurrentModel  { get; private set; }
        internal const string      TweenId       = "FORGE_TWEEN";

        public  ForgeGameConfig Config       => config;
        public  IItemService Items       => items;
        private IItemService    items;
        private ICameraService  cams;

        private IForgeGameState        state;
        
        private CancellationTokenSource cts;
        public CancellationToken CancellationToken => cts?.Token ?? CancellationToken.None;
        
        public bool IsGameActive { get; private set; }
        public bool IsPaused     { get; private set; }

        void Awake()
        {
            items = GService.GetService<IItemService>();
            cams  = GService.GetService<ICameraService>();
        }

        void Update() => state?.Update();

        #region === state-machine helpers =================================
        internal async UniTask SetState(IForgeGameState s)
        {
            if (state != null) await state.Exit();
            state = s;
            if (state != null) await state.Enter();
        }

        internal UniTask ToRecipeSelection() =>
            SetState(new ForgeRecipeSelectionState(this));

        internal UniTask ToPlayingState() =>
            SetState(new ForgePlayingState(this));

        internal UniTask ToFinalCamera(bool success) =>
            SetState(new ForgeFinalCameraState(this, success));

        internal UniTask ToResultState(string msg) =>
            SetState(new ForgeResultState(this, msg));
        #endregion

        #region === camera helpers ========================================
        public UniTask CameraToStart(CancellationToken t) =>
            camStart ? cams.MoveCamera(camStart, 1f, t, TweenId) : UniTask.CompletedTask;

        public UniTask CameraToFinal(CancellationToken t) =>
            camFinal ? cams.MoveCamera(camFinal, 1.1f, t, TweenId) : UniTask.CompletedTask;
        #endregion

        #region === product model =========================================
        public async UniTask SpawnProductModel(CancellationToken t)
        {
            if (CurrentModel) Destroy(CurrentModel.gameObject);

            var prefab = CurrentRecipe.ProductPrefab;
            if (!prefab) { Debug.LogWarning("Forge: prefab missing"); return; }

            CurrentModel = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            CurrentModel.ResetBlend();
            CurrentModel.transform.localScale = Vector3.zero;

            await CurrentModel.transform
                 .DOScale(1, .35f).SetEase(Ease.OutBack)
                 .SetId(TweenId).ToUniTask(cancellationToken: t);

            targetSpawner.SetModel(CurrentModel.Renderer);
        }

        public UniTask MoveProductToPivot(CancellationToken t)
        {
            if (!productPivot || !CurrentModel) return UniTask.CompletedTask;

            var tr = CurrentModel.transform;
            return DOTween.Sequence()
                .Append(tr.DOMove(productPivot.position, 1f).SetEase(Ease.InOutQuad))
                .Join(tr.DORotateQuaternion(productPivot.rotation, 1f).SetEase(Ease.InOutQuad))
                .SetId(TweenId).ToUniTask(cancellationToken: t);
        }
        #endregion

        #region === reward ================================================
        internal void GrantReward()
        {
            var item = CurrentRecipe.ResultItem;
            if (!items.IsUnlocked(item)) items.TryUnlock(item);
            else {
            }
        }
        #endregion

        #region === IMiniGameManager ======================================
        public async UniTask StartGame()
        {
            if (IsGameActive) return;
            IsGameActive = true; IsPaused = false;
            cts = new CancellationTokenSource();
            await ToRecipeSelection();
        }

        public async UniTask StopGame()
        {
            if (!IsGameActive) return;

            cts.Cancel();
            await SetState(null);

            hud.HideAll();
            recipePanel.Hide();
            targetSpawner.ClearAll();

            if (CurrentModel) Destroy(CurrentModel.gameObject);
            CurrentModel = null;
            IsGameActive = false;
            
            var evtManager = GService.GetService<EventsService>();
            if (evtManager && evtManager.IsEventPlaying)
            {
                evtManager.CompleteCurrentEvent();
            }
        }

        public UniTask PauseGame()  { DOTween.Pause(TweenId); IsPaused = true;  return UniTask.CompletedTask; }
        public UniTask ResumeGame() { DOTween.Play (TweenId); IsPaused = false; return UniTask.CompletedTask; }
        public async UniTask RestartGame() { await StopGame(); await StartGame(); }
        #endregion

        #region === getters for states ====================================
        public ForgeRecipePanel  RecipeUI => recipePanel;
        public ForgeTargetSpawner Spawner => targetSpawner;
        public ForgeHUD           HUD     => hud;
        #endregion
    }
}
