using _Core.GameEvents.Forging.Scripts;
using Cysharp.Threading.Tasks;

namespace _Core.GameEvents.Forging.States
{
    public sealed class ForgeResultState : IForgeGameState
    {
        private readonly ForgeGameManager mgr;
        private readonly string           message;

        public ForgeResultState(ForgeGameManager m, string msg)
        { mgr = m; message = msg; }

        public UniTask Enter()
        {
            mgr.HUD.ShowResult(
                message,
                onMore   : () => mgr.ToRecipeSelection().Forget(),
                onFinish : () => mgr.StopGame().Forget());

            return UniTask.CompletedTask;
        }

        public void   Update() { }
        public UniTask Exit()  { mgr.HUD.HideAll(); return UniTask.CompletedTask; }
    }
}