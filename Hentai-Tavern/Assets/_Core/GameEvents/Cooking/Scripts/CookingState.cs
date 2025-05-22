// CookingState.cs
using System.Threading;
using Cysharp.Threading.Tasks;
using _Core.GameEvents.Cooking.Scripts.HUD;
using UnityEngine;

namespace _Core.GameEvents.Cooking.Scripts.States
{
    public class CookingState : ICookingGameState
    {
        private readonly CookingGameManager mgr;
        private readonly CookingHUD         hud;
        private float                       timer;

        public CookingState(CookingGameManager mgr, CookingHUD hud)
        {
            this.mgr = mgr;
            this.hud = hud;
        }

        public async UniTask Enter(CancellationToken cancellationToken)
        {
            hud.HideAll();
            timer = 0f;
            hud.CookPanel.SetProgress(0f);
            hud.CookPanel.Show();
            await mgr.MoveCam(mgr.CookingCamPoint, "Cook_Pot");
        }

        public void Tick()
        {
            timer += Time.deltaTime;
            float norm = Mathf.Clamp01(timer / mgr.Config.CookingDuration);
            hud.CookPanel.SetProgress(norm);
            if (norm >= 1f)
                mgr.SwitchState(mgr.ServingState).Forget();
        }

        public UniTask Exit(CancellationToken cancellationToken)
        {
            hud.HideAll();
            return UniTask.CompletedTask;
        }
    }
}