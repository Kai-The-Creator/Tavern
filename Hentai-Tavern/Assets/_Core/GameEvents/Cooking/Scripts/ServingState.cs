// ServingState.cs
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using _Core.GameEvents.Cooking.Scripts.HUD;
using UnityEngine;

namespace _Core.GameEvents.Cooking.Scripts.States
{
    public class ServingState : ICookingGameState
    {
        private readonly CookingGameManager mgr;
        private readonly CookingHUD         hud;
        private CookingDishObject           spawned;

        public ServingState(CookingGameManager mgr, CookingHUD hud)
        {
            this.mgr = mgr;
            this.hud = hud;
        }

        public async UniTask Enter(CancellationToken cancellationToken)
        {
            hud.HideAll();
            await mgr.MoveCam(mgr.ServingCamPoint, "Cook_Serving");
            hud.ServingPanel.Show();
            
            spawned = Object.Instantiate(
                mgr.CurrentOrder.DishPrefab,
                mgr.DishServingPoint.position,
                Quaternion.identity
            ).GetComponent<CookingDishObject>();
            
            hud.ServingPanel.ServeClicked += OnServeClicked;
        }

        private void OnServeClicked()
        {
            spawned.Serve(0.4f).OnComplete(() => Object.Destroy(spawned.gameObject));

            // spawned = Object.Instantiate(
            //     mgr.CurrentOrder.DishPrefab,
            //     mgr.DishServingPoint.position,
            //     mgr.DishServingPoint.rotation);
            //
            // spawned.transform.DOScale(0f, .4f).OnComplete(
            //     () => Object.Destroy(spawned.gameObject));
            
            mgr.MarkOrderServed();
            mgr.SwitchState(mgr.ResultState).Forget();
        }

        public void Tick() { }

        public UniTask Exit(CancellationToken cancellationToken)
        {
            hud.ServingPanel.ServeClicked -= OnServeClicked;
            hud.HideAll();
            return UniTask.CompletedTask;
        }
    }
}