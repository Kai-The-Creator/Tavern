using _Core._Global.CameraService;
using _Core._Global.Services;
using _Core._Global.UISystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core.GameEvents.Forging.Scripts
{
    public class ForgeGameEvent : GameEvent
    {
        [Header("Root of Forge mini-game")]
        [SerializeField] private GameObject forgeRoot;

        private ForgeGameManager mgr;

        protected override void Activate()
        {
            base.Activate();
            forgeRoot.SetActive(true);
            GService.GetService<IUIService>().CloseAllWindows();

            mgr ??= forgeRoot.GetComponentInChildren<ForgeGameManager>(true);
            mgr.StartGame().Forget();
        }

        protected override async void Deactivate()
        {
            forgeRoot.SetActive(false);
            await GService.GetService<ICameraService>().MoveCameraToStartPosition();
            GService.GetService<IUIService>().ShowWindow(WindowType.Widget);
            base.Deactivate();
        }
    }
}