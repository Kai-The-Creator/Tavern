using System.Threading;
using _Core._Global.CameraService;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using _Core._Global.Services;
using _Core._Global.ItemSystem;
using _Core._Global.WalletSystem;
using _Core.GameEvents.Common;
using _Core.GameEvents.Enchantment.UI;
using _Core.GameEvents.Enchantment.Data;
using _Core.GameEvents.Enchantment.States;

namespace _Core.GameEvents.Enchantment.Scripts
{
    public sealed class EnchantmentGameManager : MonoBehaviour, IMiniGameManager
    {
        [Header("Config & UI")]
        [SerializeField] private EnchantmentGameConfig config;
        [SerializeField] private EnchantmentHUD       ui;

        [Header("Camera")]
        [SerializeField] private Transform camStart;
        [SerializeField] private Transform camFinal;

        [Header("Spawn")]
        [SerializeField] private Transform productPivot;

        [Header("Station tag")]
        [SerializeField] private WorkstationTag magicTag;

        private IItemService   items;
        public  IItemService   ItemSvc => items;
        private IWalletService wallet;
        private ICameraService cams;

        private IEnchantmentState state;
        private CancellationTokenSource cts;
        public  bool   IsGameActive{get;private set;}
        public  bool   IsPaused    {get;private set;}
        public  CraftRecipeConfig CurrentRecipe{get;set;}

        private ProductModel currentModel;
        internal EnchantmentGameConfig Config => config;

        private void Awake()
        {
            items  = GService.GetService<IItemService>();
            wallet = GService.GetService<IWalletService>();
            cams   = GService.GetService<ICameraService>();

        }

        private void Update() => state?.Update();

        #region State change
        internal async UniTask ChangeState(IEnchantmentState s)
        {
            if (state!=null) await state.Exit();
            state = s;
            if (state!=null) await state.Enter();
        }
        #endregion

        #region Camera / model
        internal UniTask CameraToStart(CancellationToken t)=> cams.MoveCamera(camStart,1f,t,"EnchantTween");
        internal UniTask CameraToFinal(CancellationToken t)=> cams.MoveCamera(camFinal,1f,t,"EnchantTween");

        internal async UniTask SpawnProduct(CancellationToken t)
        {
            if (currentModel) Destroy(currentModel.gameObject);
            if (!CurrentRecipe.ProductPrefab) return;
            currentModel = Instantiate(CurrentRecipe.ProductPrefab, productPivot.position, productPivot.rotation);
            currentModel.ResetBlend();
            currentModel.transform.localScale = Vector3.zero;
            await currentModel.transform
                .DOScale(1f,.4f).SetEase(Ease.OutBack).SetId("EnchantTween")
                .ToUniTask(cancellationToken:t);
        }
        #endregion

        #region Reward
        internal void GrantReward(bool success)
        {
            if (!success) return;

            var item = CurrentRecipe.ResultItem;
            if (!items.IsUnlocked(item))
                items.TryUnlock(item);
            else
                wallet.Earn(new Money("Gold", config.duplicateGold));
        }
        #endregion

        #region IMiniGameManager
        public async UniTask StartGame()
        {
            if (IsGameActive) return;
            ui.Init(items, magicTag, config.RunePool);
            IsGameActive=true; IsPaused=false;
            cts = new CancellationTokenSource();
            await ChangeState(new RecipeSelectionState(this, ui, cts));
        }

        public UniTask PauseGame(){ DOTween.Pause("EnchantTween"); IsPaused=true; return UniTask.CompletedTask; }
        public UniTask ResumeGame(){ DOTween.Play("EnchantTween"); IsPaused=false; return UniTask.CompletedTask; }

        public async UniTask StopGame()
        {
            if (!IsGameActive) return;
            cts.Cancel();
            await ChangeState(null);
            if (currentModel) Destroy(currentModel.gameObject);
            ui.HideAll();
            IsGameActive=false;
            
            var evtManager = GService.GetService<EventsService>();
            if (evtManager && evtManager.IsEventPlaying)
            {
                evtManager.CompleteCurrentEvent();
            }
        }

        public UniTask RestartGame()=> StopGame().ContinueWith(StartGame);
        public async UniTask ExitGame(){ await StopGame(); }
        #endregion
    }
}
