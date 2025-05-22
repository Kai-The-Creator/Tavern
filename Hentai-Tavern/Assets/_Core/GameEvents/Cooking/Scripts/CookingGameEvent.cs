// CookingGameEvent.cs
using _Core._Global.CameraService;
using _Core._Global.Services;
using _Core._Global.UISystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core.GameEvents.Cooking.Scripts
{
    public class CookingGameEvent : GameEvent
    {
        [Header("Root of Cooking mini-game")]
        [SerializeField] private GameObject cookingRoot;

        private CookingGameManager manager;

        protected override void Activate()
        {
            base.Activate();
            cookingRoot.SetActive(true);
            GService.GetService<IUIService>().CloseAllWindows();

            manager ??= cookingRoot.GetComponentInChildren<CookingGameManager>(true);
            manager.StartGame().Forget();
        }

        protected override async void Deactivate()
        {
            cookingRoot.SetActive(false);
            await GService.GetService<ICameraService>().MoveCameraToStartPosition();
            GService.GetService<IUIService>().ShowWindow(WindowType.Widget);
            base.Deactivate();
        }
    }
}