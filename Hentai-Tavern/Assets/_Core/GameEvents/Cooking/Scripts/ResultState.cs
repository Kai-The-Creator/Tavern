// ResultState.cs
using System.Threading;
using Cysharp.Threading.Tasks;
using _Core.GameEvents.Cooking.Scripts.HUD;
using UnityEngine;

namespace _Core.GameEvents.Cooking.Scripts.States
{
    public class ResultState : ICookingGameState
    {
        private readonly CookingGameManager mgr;
        private readonly CookingHUD         hud;

        public ResultState(CookingGameManager mgr, CookingHUD hud)
        {
            this.mgr = mgr;
            this.hud = hud;
        }

        public async UniTask Enter(CancellationToken cancellationToken)
        {
            hud.HideAll();
            await mgr.MoveCam(mgr.ResultCamPoint, "Cook_Result");
            hud.ResultPanel.Show();
            hud.ResultPanel.ContinueClicked += OnContinue;
            hud.ResultPanel.ExitClicked     += OnExit;
        }

        private void OnContinue()
        {
            hud.ResultPanel.ContinueClicked -= OnContinue;
            hud.ResultPanel.ExitClicked     -= OnExit;
            mgr.SwitchState(mgr.SelectingState).Forget();
        }

        private void OnExit()
        {
            hud.ResultPanel.ContinueClicked -= OnContinue;
            hud.ResultPanel.ExitClicked     -= OnExit;
            mgr.StopGame().Forget();
        }

        public void Tick() { }

        public UniTask Exit(CancellationToken cancellationToken)
        {
            hud.HideAll();
            return UniTask.CompletedTask;
        }
    }
}