using _Core._Global.CameraService;
using _Core._Global.Services;
using _Core._Global.UISystem;
using _Core.GameEvents.Forging.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core.GameEvents.Enchantment.Scripts
{
    public class EnchantmentGameEvent : GameEvent
    {
        [SerializeField] private GameObject enchantRoot;
        private EnchantmentGameManager mgr;

        protected override void Activate()
        {
            base.Activate();
            enchantRoot.SetActive(true);
            GService.GetService<IUIService>().CloseAllWindows();

            mgr ??= enchantRoot.GetComponentInChildren<EnchantmentGameManager>(true);
            mgr.StartGame().Forget();
        }

        protected override async void Deactivate()
        {
            enchantRoot.SetActive(false);
            await GService.GetService<ICameraService>().MoveCameraToStartPosition();
            GService.GetService<IUIService>().ShowWindow(WindowType.Widget);
            base.Deactivate();
        }
    }
}