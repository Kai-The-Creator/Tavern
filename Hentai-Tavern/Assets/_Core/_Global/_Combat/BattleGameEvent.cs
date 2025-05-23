using _Core._Global.CameraService;
using _Core._Global.Services;
using _Core._Global.UISystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core.GameEvents.Battle
{
    public class BattleGameEvent : GameEvent
    {
        [SerializeField] private GameObject battleRoot;
        private BattleGameManager manager;

        protected override void Activate()
        {
            base.Activate();
            Debug.Log("Battle Game Event Activated");
            battleRoot.SetActive(true);
            GService.GetService<IUIService>().CloseAllWindows();

            manager ??= battleRoot.GetComponentInChildren<BattleGameManager>(true);
            manager.StartGame().Forget();
        }

        protected override async void Deactivate()
        {
            Debug.Log("Battle Game Event DeActivated");
            battleRoot.SetActive(false);
            await GService.GetService<ICameraService>().MoveCameraToStartPosition();
            GService.GetService<IUIService>().ShowWindow(WindowType.Widget);
            base.Deactivate();
        }
    }
}