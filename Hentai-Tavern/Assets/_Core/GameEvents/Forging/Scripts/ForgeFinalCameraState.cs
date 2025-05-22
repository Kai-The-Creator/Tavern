using System.Threading;
using _Core.GameEvents.Forging.Scripts;
using Cysharp.Threading.Tasks;

namespace _Core.GameEvents.Forging.States
{
    public sealed class ForgeFinalCameraState : IForgeGameState
    {
        private readonly ForgeGameManager mgr;
        private readonly bool             success;
        private readonly CancellationToken token;

        public ForgeFinalCameraState(ForgeGameManager m, bool succ)
        {
            mgr   = m;
            success = succ;
            token  = m.CancellationToken;
        }

        public async UniTask Enter()
        {
            await mgr.CameraToFinal(token);
            await mgr.MoveProductToPivot(token);

            if (success) mgr.GrantReward();
            string msg = success ? "Success!" : "Fail! Materials lost.";
            await mgr.ToResultState(msg);
        }

        public void   Update(){}
        public UniTask Exit() => UniTask.CompletedTask;
    }
}