using System.Threading;
using _Core.GameEvents.Enchantment.Scripts;
using Cysharp.Threading.Tasks;
using _Core.GameEvents.Enchantment.UI;

namespace _Core.GameEvents.Enchantment.States
{
    public sealed class FinalResultState : IEnchantmentState
    {
        private readonly EnchantmentGameManager mgr;
        private readonly EnchantmentHUD ui;
        private readonly CancellationTokenSource cts;
        private readonly bool victory;

        public FinalResultState(EnchantmentGameManager m, EnchantmentHUD u,
            CancellationTokenSource c, bool v)
        { mgr=m; ui=u; cts=c; victory=v; }

        public UniTask Enter()
        {
            string msg = victory
                ? $"Success! Crafted {mgr.CurrentRecipe.ResultItem.DisplayName}"
                : "Fail! Materials spent.";

            ui.ShowResult(
                msg,
                onMore:   () => mgr.ChangeState(new RecipeSelectionState(mgr, ui, cts)).Forget(),
                onFinish: () => mgr.ExitGame().Forget());

            return UniTask.CompletedTask;
        }

        public void Update(){}
        public UniTask Exit(){ ui.HideAll(); return UniTask.CompletedTask; }
    }
}