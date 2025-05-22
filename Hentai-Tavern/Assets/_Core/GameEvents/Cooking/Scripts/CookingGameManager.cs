// CookingGameManager.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using _Core._Global.CameraService;
using _Core._Global.ItemSystem;
using _Core._Global.Services;
using _Core.GameEvents.Cooking.Scripts.HUD;
using _Core.GameEvents.Cooking.Scripts.States;
using UnityEngine;

namespace _Core.GameEvents.Cooking.Scripts
{
    public class CookingGameManager : MonoBehaviour, IMiniGameManager
    {
        [Header("Config & refs")]
        [SerializeField] public CookingGameConfig  Config;
        [SerializeField] private CookingHUD        hud;

        [Header("Camera points")]
        [SerializeField] public Transform SelectionCamPoint;
        [SerializeField] public Transform CuttingCamPoint;
        [SerializeField] public Transform CookingCamPoint;
        [SerializeField] public Transform ServingCamPoint;
        [SerializeField] public Transform ResultCamPoint;

        [Header("Serving")]
        [SerializeField] public Transform DishServingPoint;
        [SerializeField] public Transform SpawnIngredientPoint;
        [SerializeField] public Transform TrashIngredientPoint;

        #region Services
        public ICameraService CameraSvc { get; private set; }
        public IItemService    ItemSvc   { get; private set; }
        #endregion

        #region Orders / runtime
        public List<OrderRecord> Orders { get; private set; }
        public OrderRecord       CurrentOrder { get; private set; }
        private OrderButtonView  currentBtn;
        public bool              HasMoreOrders => Orders.Any(o => o.Quantity > 0);
        private CookingRuntimeData runtime;
        #endregion

        #region States
        internal SelectingOrderState SelectingState  { get; private set; }
        internal CuttingState         CuttingState    { get; private set; }
        internal CookingState         CookingState    { get; private set; }
        internal ServingState         ServingState    { get; private set; }
        internal ResultState          ResultState     { get; private set; }
        #endregion

        private ICookingGameState current;
        public CancellationTokenSource CamCTS { get; private set; }

        #region IMiniGameManager
        public bool IsGameActive { get; private set; }
        public bool IsPaused     { get; private set; }

        public async UniTask MoveCam(Transform target, string tweenId)
        {
            await CameraSvc.MoveCamera(
                target,
                Config.CameraMoveDuration,
                CamCTS.Token,
                tweenId);
        }

        public async UniTask StartGame()
        {
            if (IsGameActive) return;

            // Получаем сервисы
            CameraSvc = GService.GetService<ICameraService>();
            ItemSvc   = GService.GetService<IItemService>();

            // Сбрасываем токен
            CamCTS?.Cancel();
            CamCTS?.Dispose();
            CamCTS = new CancellationTokenSource();

            // Генерация заказов и инициализация(Runtime)
            Orders  = Config.GenerateOrders();
            runtime = new CookingRuntimeData();
            runtime.InitFromGlobal(
                ItemSvc,
                Orders.SelectMany(o => o.RequiredIngredients).Select(r => r.Ingredient)
            );

            BuildStates();
            IsGameActive = true;

            // Переходим в первое состояние
            await SwitchState(SelectingState);
        }

        public UniTask PauseGame()
        {
            IsPaused      = true;
            Time.timeScale = 0f;
            return UniTask.CompletedTask;
        }

        public UniTask ResumeGame()
        {
            IsPaused      = false;
            Time.timeScale = 1f;
            return UniTask.CompletedTask;
        }

        public async UniTask StopGame()
        {
            if (!IsGameActive) return;

            CamCTS.Cancel();
            try
            {
                await current.Exit(CamCTS.Token);
            }
            catch (OperationCanceledException)
            {
                // отмена перехода — игнорируем
            }

            IsGameActive = false;
            hud.HideAll();
        }

        public UniTask RestartGame() => StopGame().ContinueWith(StartGame);
        #endregion

        private void Update()
        {
            if (IsGameActive && !IsPaused)
                current?.Tick();
        }

        private void BuildStates()
        {
            SelectingState = new SelectingOrderState(this, hud);
            CuttingState   = new CuttingState(this, hud, runtime);
            CookingState   = new CookingState(this, hud);
            ServingState   = new ServingState(this, hud);
            ResultState    = new ResultState(this, hud);
        }

        /// <summary>
        /// Переключает состояния, корректно обрабатывая отмену через токен.
        /// </summary>
        internal async UniTask SwitchState(ICookingGameState next)
        {
            try
            {
                if (current != null)
                    await current.Exit(CamCTS.Token);

                current = next;
                await current.Enter(CamCTS.Token);
            }
            catch (OperationCanceledException)
            {
                // если отменили — просто выходим
            }
        }

        #region Public API for States
        /// <summary>
        /// Выбор заказа: резервируем единицу, обновляем кнопку и показываем панель.
        /// </summary>
        public void HandleOrderClicked(OrderButtonView btn)
        {
            // Если уже выбрали именно эту кнопку — ничего не делаем
            if (btn == currentBtn) 
                return;

            // Если был ранее выбран другой заказ — возвращаем его в пул
            if (currentBtn != null)
            {
                currentBtn.Record.Quantity++;
                currentBtn.Refresh();
            }

            // Резервируем новую единицу текущего заказа
            btn.Record.Quantity--;
            btn.Refresh();

            // Сохраняем новую текущую кнопку/заказ
            currentBtn   = btn;
            CurrentOrder = btn.Record;

            // Обновляем и показываем панель выбранного заказа
            hud.SelectedOrderPanel.Bind(CurrentOrder);
            hud.SelectedOrderPanel.Show();

            // Включаем/отключаем кнопку старта по наличию ингредиентов
            ValidateStartButton();
        }

        /// <summary>
        /// Отмена выбора: возвращаем зарезервированную единицу и обновляем UI.
        /// </summary>
        public void CancelCurrentSelection()
        {
            if (currentBtn == null) return;

            currentBtn.Record.Quantity++;
            currentBtn.Refresh();

            currentBtn   = null;
            CurrentOrder = null;

            hud.SelectedOrderPanel.Clear();
            ValidateStartButton();
        }

        public void SelectOrder(OrderRecord rec) => CurrentOrder = rec;

        public void ValidateStartButton()
        {
            if (CurrentOrder == null)
            {
                hud.OrdersPanel.SetStartInteractable(false, null);
                return;
            }

            bool canCook = CurrentOrder.RequiredIngredients.All(
                r => ItemSvc.GetQuantity(r.Ingredient) >= r.Quantity
            );

            hud.OrdersPanel.SetStartInteractable(canCook, OnStartCookingClicked);
        }

        private void OnStartCookingClicked() => SwitchState(CuttingState).Forget();
        #endregion

        /// <summary>
        /// Анимация нарезки одного ингредиента.
        /// </summary>
        public async UniTask SpawnAndChop(IngredientConfig cfg)
        {
            var obj = Instantiate(
                cfg.RenderObject,
                SpawnIngredientPoint.position,
                Quaternion.identity
            );

            await obj.Chop(
                cfg.ChopClickCount,
                TrashIngredientPoint,
                CameraSvc.GetMainCamera()
            );

            Destroy(obj.gameObject);
        }

        /// <summary>
        /// После подачи проверяем, нужно ли убрать заказ полностью.
        /// </summary>
        public void MarkOrderServed()
        {
            if (CurrentOrder.Quantity <= 0)
                Orders.Remove(CurrentOrder);
        }
    }
}
