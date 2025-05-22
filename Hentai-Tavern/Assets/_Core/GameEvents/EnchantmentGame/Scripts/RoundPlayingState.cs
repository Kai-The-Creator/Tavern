using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Core.GameEvents.Enchantment.Data;
using _Core.GameEvents.Enchantment.Scripts;
using _Core.GameEvents.Enchantment.UI;

namespace _Core.GameEvents.Enchantment.States
{
    public sealed class RoundPlayingState : IEnchantmentState
    {
        private readonly EnchantmentGameManager mgr;
        private readonly EnchantmentHUD hud;
        private readonly CancellationTokenSource cts;

        private EnchantmentRoundManager roundMgr;

        private const float roundPause = .6f;

        public RoundPlayingState(EnchantmentGameManager m, EnchantmentHUD h, CancellationTokenSource c)
        { mgr = m; hud = h; cts = c; }

        public async UniTask Enter()
        {
            var cfg  = mgr.Config;
            var item = mgr.ItemSvc;

            foreach (var (res, cnt) in mgr.CurrentRecipe.Materials)
                item.TryConsume(res, cnt);

            roundMgr = new EnchantmentRoundManager(cfg.RunePool,
                                                   cfg.minSequenceLength,
                                                   cfg.maxSequenceLength,
                                                   cfg.maxConsecutiveRepeat);

            int lives = cfg.infiniteLives ? int.MaxValue : cfg.totalLives;
            int done  = 0;

            hud.ShowGame(cfg.roundsToWin, lives, cfg.infiniteLives);

            while (true)
            {
                hud.LockRunes(true);
                for (int i = 0; i < cfg.RunePool.Length; i++)
                    hud.BindRune(i, cfg.RunePool[i], null);

                foreach (var rune in roundMgr.Sequence)
                {
                    var rv = hud.GetRune(System.Array.IndexOf(cfg.RunePool, rune));
                    await rv.Flash(cfg.runeShowTime);
                    await UniTask.Delay(Mathf.RoundToInt(cfg.runePauseTime * 1000), cancellationToken:cts.Token);
                }

                hud.LockRunes(false);

                var tcs = new UniTaskCompletionSource<bool>();
                int expectIndex = 0;

                void HandleClick(RuneView rv)
                {
                    if (expectIndex >= roundMgr.Sequence.Count || rv.Rune != roundMgr.Sequence[expectIndex])
                    {
                        tcs.TrySetResult(false);
                        return;
                    }

                    expectIndex++;
                    if (expectIndex == roundMgr.Sequence.Count)
                        tcs.TrySetResult(true);
                }

                for (int i = 0; i < cfg.RunePool.Length; i++)
                    hud.GetRune(i)?.SetCallback(HandleClick);

                var timeout = UniTask.Delay(Mathf.RoundToInt(cfg.inputTimeout * 1000), cancellationToken:cts.Token);
                var (idx, success) = await UniTask.WhenAny(tcs.Task, timeout);

                for (int i = 0; i < cfg.RunePool.Length; i++)
                    hud.GetRune(i)?.SetCallback(null);
                hud.LockRunes(true);

                if (!success && lives != int.MaxValue)
                {
                    lives--;
                    done = 0;
                    hud.TweenRound(done, .25f);
                }
                if (success)
                {
                    done++;
                    await hud.TweenRound(done, .25f);
                }

                hud.UpdateLives(lives, cfg.infiniteLives);

                if (lives == 0 || done >= cfg.roundsToWin) break;

                roundMgr.NextSequence();
                await UniTask.Delay(Mathf.RoundToInt(roundPause*1000),cancellationToken: cts.Token);
            }

            await mgr.ChangeState(new FinalResultState(mgr, hud, cts, lives > 0));
        }

        public void Update() { }

        public UniTask Exit()
        {
            hud.ClearRunes();
            return UniTask.CompletedTask;
        }
    }
}
