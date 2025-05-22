using System.Threading;
using _Core._Global.ItemSystem;
using _Core.GameEvents.Enchantment.Scripts;
using Cysharp.Threading.Tasks;
using _Core.GameEvents.Enchantment.UI;
using UnityEngine;

namespace _Core.GameEvents.Enchantment.States
{
    public sealed class RecipeSelectionState : IEnchantmentState
    {
        private readonly EnchantmentGameManager mgr;
        private readonly EnchantmentHUD ui;
        private readonly CancellationTokenSource cts;

        public RecipeSelectionState(EnchantmentGameManager m, EnchantmentHUD u, CancellationTokenSource c){mgr=m; ui=u; cts=c;}

        public async UniTask Enter()
        {
            ui.ShowRecipe();
            CraftRecipeConfig rec;
            try { rec = await ui.WaitForRecipe(cts.Token); }
            catch { await mgr.ExitGame(); return; }

            mgr.CurrentRecipe = rec;
            await mgr.ChangeState(new RoundPlayingState(mgr, ui, cts));
        }
        public void Update(){}
        public UniTask Exit(){ ui.HideAll(); return UniTask.CompletedTask; }
    }
}