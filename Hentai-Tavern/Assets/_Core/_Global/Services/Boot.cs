using System;
using System.Threading;
using _Core._Global.UISystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Core._Global.Services
{
    public class Boot : MonoBehaviour
    {
        [Header("ServicesInitializer")] [SerializeField]
        private ServicesInitializer initializer;

        [Header("Scene to load after init")] [SerializeField]
        private GameScenes gameScene;

        [Header("Extra wait time (seconds) after init")] [SerializeField]
        private double sceneLoadTime = 1.0;

        [Header("Init timeout (seconds), <0 — no timeout)")] [SerializeField]
        private float initTimeout = 10f;

        private CancellationTokenSource _timeoutCts;

        private void Awake()
        {
            _timeoutCts = initTimeout > 0
                ? new CancellationTokenSource(TimeSpan.FromSeconds(initTimeout))
                : new CancellationTokenSource();
        }

        private async void Start()
        {
            try
            {
                await UniTask.WaitUntil(
                    () => initializer != null && initializer.IsInitialized,
                    cancellationToken: _timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Boot: service initialization timed out!");
                // TODO: показать кнопку Retry/Exit
                return;
            }

            var ui = GService.GetService<IUIService>();
            ui?.ShowWindow(WindowType.Initializer, WindowLayer.Modal);

            await UniTask.Delay(TimeSpan.FromSeconds(sceneLoadTime), cancellationToken: _timeoutCts.Token);

            await SceneManager.LoadSceneAsync(gameScene.ToString(), LoadSceneMode.Additive)
                .ToUniTask(cancellationToken: _timeoutCts.Token);

            Debug.Log("Boot: main scene loaded.");

            ui?.CloseWindow(WindowType.Initializer);
        }

        private void OnDestroy()
        {
            _timeoutCts?.Cancel();
            _timeoutCts?.Dispose();
        }
    }
}