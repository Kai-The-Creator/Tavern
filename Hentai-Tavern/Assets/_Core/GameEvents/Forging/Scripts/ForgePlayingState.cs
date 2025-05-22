using System.Threading;
using _Core.GameEvents.Forging.Data;
using _Core.GameEvents.Forging.Scripts;
using _Core.GameEvents.Forging.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Core.GameEvents.Forging.States
{
    public sealed class ForgePlayingState : IForgeGameState
    {
         private readonly ForgeGameManager mgr;
        private readonly CancellationToken token;

        private int   lives;
        private float blend;

        public ForgePlayingState(ForgeGameManager m)
        {
            mgr   = m;
            token = m.CancellationToken;
        }

        public async UniTask Enter()
        {
            await mgr.CameraToStart(token);
            mgr.HUD.ShowGame();

            var cfg = mgr.Config;
            lives   = cfg.InfiniteLives ? int.MaxValue : cfg.TotalLives;
            mgr.HUD.UpdateLives(lives, cfg.InfiniteLives);

            await mgr.SpawnProductModel(token);

            blend = 0f;
            mgr.HUD.UpdateProgress(0f);

            await SpawnLoop(cfg);
            bool success = blend >= cfg.MaxBlend && lives > 0;
            await mgr.ToFinalCamera(success);
        }

        public void Update() {}
        public UniTask Exit() { mgr.Spawner.ClearAll(); mgr.HUD.HideAll(); return UniTask.CompletedTask; }

        private async UniTask SpawnLoop(ForgeGameConfig cfg)
        {
            while (blend < cfg.MaxBlend && lives > 0 && !token.IsCancellationRequested)
            {
                if (mgr.Spawner.ActiveCount < cfg.MaxTargets)
                    _ = HandleOneTarget(cfg);

                await UniTask.Delay(RandomDelay(cfg), cancellationToken: token);
            }
        }

        private async UniTask HandleOneTarget(ForgeGameConfig cfg)
        {
            if (!mgr.Spawner.TrySpawn(out var tv, out _)) return;

            tv.Init();
            bool clicked = await tv.WaitForClickOrTimeout(cfg.TargetLifetime, token);

            mgr.Spawner.Despawn(tv);

            if (!clicked)
            {
                if (lives != int.MaxValue) lives--;
                mgr.HUD.UpdateLives(lives, cfg.InfiniteLives);
                return;
            }
            AddBlend(cfg.PointsFirstHalf, cfg);
        }

        private int RandomDelay(ForgeGameConfig cfg) => Mathf.RoundToInt(Random.Range(cfg.MinSpawnDelay, cfg.MaxSpawnDelay)*1000);

        private void AddBlend(int points, ForgeGameConfig cfg)
        {
            float add = points * cfg.BlendPerPoint;
            blend     = Mathf.Min(blend + add, cfg.MaxBlend);

            mgr.CurrentModel.SetBlend(blend);
            float n01 = blend / cfg.MaxBlend;

            mgr.HUD.TweenProgress(n01, .25f).Forget();
        }
    }
}
