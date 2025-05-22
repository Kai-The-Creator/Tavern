// CuttingState.cs
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using _Core._Global.ItemSystem;
using _Core.GameEvents.Cooking.Scripts.HUD;

namespace _Core.GameEvents.Cooking.Scripts.States
{
    public class CuttingState : ICookingGameState
    {
        private readonly CookingGameManager mgr;
        private readonly CookingHUD         hud;
        private readonly CookingRuntimeData runtime;

        // локальные данные по нарезке
        private Dictionary<IngredientConfig, int> need;
        private int totalToCut;
        private int cutCount;
        private bool isChopping;
        private CancellationToken ct;

        public CuttingState(
            CookingGameManager mgr,
            CookingHUD hud,
            CookingRuntimeData runtime)
        {
            this.mgr     = mgr;
            this.hud     = hud;
            this.runtime = runtime;
        }

        public async UniTask Enter(CancellationToken cancellationToken)
        {
            ct = cancellationToken;

            // готовим данные
            need       = mgr.CurrentOrder.RequiredIngredients
                            .ToDictionary(r => r.Ingredient, r => r.Quantity);
            totalToCut = need.Values.Sum();
            cutCount   = 0;
            isChopping = false;

            // камера + UI
            hud.HideAll();
            await mgr.MoveCam(mgr.CuttingCamPoint, "Cook_Cut");
            hud.SelectedOrderPanel.Bind(mgr.CurrentOrder);
            hud.SelectedOrderPanel.Show();
            hud.CuttingPanel.BuildInventory(runtime.Stock, OnIngredientClicked);

            // стартовая заливка прогресса
            hud.CuttingPanel.SetProgress(0f);
        }

        public void Tick()
        {
            // ничего: ходим вручную по кликам
        }

        public async UniTask Exit(CancellationToken cancellationToken)
        {
            hud.HideAll();
        }

        private async void OnIngredientClicked(IngredientConfig cfg)
        {
            if (isChopping)                                    return;
            if (!need.TryGetValue(cfg, out var toCut) || toCut <= 0) return;
            if (runtime.Stock[cfg] <= 0)                       return;

            isChopping = true;

            try
            {
                // анимация/нарезка
                await mgr.SpawnAndChop(cfg).AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                isChopping = false;
                return;
            }

            // списываем ингредиент из стока
            runtime.TryConsume(cfg, 1, mgr.ItemSvc);
            need[cfg]--;
            cutCount++;

            // обновляем два UI: инвентарь и панель заказа
            hud.CuttingPanel.UpdateAmount(cfg, runtime.Stock[cfg]);
            hud.SelectedOrderPanel.View.DecrementIngredient(cfg);  // :contentReference[oaicite:0]{index=0}:contentReference[oaicite:1]{index=1}

            // обновляем прогресс-бар
            hud.CuttingPanel.SetProgress((float)cutCount / totalToCut);

            // проверяем, всё ли нарезано
            if (need.Values.All(v => v == 0))
            {
                await mgr.SwitchState(mgr.CookingState);
            }

            isChopping = false;
        }
    }
}
