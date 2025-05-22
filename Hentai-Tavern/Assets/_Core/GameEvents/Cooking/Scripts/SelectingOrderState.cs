// SelectingOrderState.cs
using System.Threading;
using Cysharp.Threading.Tasks;
using _Core.GameEvents.Cooking.Scripts.HUD;

namespace _Core.GameEvents.Cooking.Scripts.States
{
    public class SelectingOrderState : ICookingGameState
    {
        private readonly CookingGameManager mgr;
        private readonly CookingHUD        hud;

        public SelectingOrderState(CookingGameManager mgr, CookingHUD hud)
        {
            this.mgr = mgr;
            this.hud = hud;
        }

        private void OnOrderClicked(OrderButtonView view)
        {
            // Delegate to manager to handle selection logic and UI updates
            mgr.HandleOrderClicked(view);
        }

        private void OnCancelSelection()
        {
            mgr.CancelCurrentSelection();
        }

        public async UniTask Enter(CancellationToken cancellationToken)
        {
            hud.HideAll();
            await mgr.MoveCam(mgr.SelectionCamPoint, "Cook_Select");

            // Build buttons with updated handler
            hud.OrdersPanel.BuildButtons(mgr.Orders, OnOrderClicked);
            hud.SelectedOrderPanel.CancelClicked += OnCancelSelection;
        }

        public void Tick()
        {
            // No per-frame logic needed here
        }

        public UniTask Exit(CancellationToken cancellationToken)
        {
            hud.SelectedOrderPanel.CancelClicked -= OnCancelSelection;
            hud.HideAll();
            return UniTask.CompletedTask;
        }
    }
}