using System.Threading;
using _Core.GameEvents.Forging.Scripts;
using Cysharp.Threading.Tasks;

namespace _Core.GameEvents.Forging.States
{
    public sealed class ForgeRecipeSelectionState : IForgeGameState
    {
        private readonly ForgeGameManager     mgr;
        private readonly CancellationToken    token;

        public ForgeRecipeSelectionState(ForgeGameManager m)
        {
            mgr   = m;
            token = m.CancellationToken;
        }

        public async UniTask Enter()
        {
            await mgr.CameraToStart(token);

            mgr.RecipeUI.Show();
            var recipe = await mgr.RecipeUI.WaitForChoice(token);
            mgr.CurrentRecipe = recipe;

            foreach (var (res, cnt) in recipe.Materials)
                mgr.Items.TryConsume(res, cnt);
            
            await mgr.ToPlayingState();
        }

        public void   Update() {}
        public UniTask Exit()  { mgr.RecipeUI.Hide(); return UniTask.CompletedTask; }
    }
}