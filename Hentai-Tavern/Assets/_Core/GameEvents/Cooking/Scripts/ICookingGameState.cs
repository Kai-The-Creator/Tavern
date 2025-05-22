// ICookingGameState.cs
using Cysharp.Threading.Tasks;
using System.Threading;

namespace _Core.GameEvents.Cooking.Scripts.States
{
    public interface ICookingGameState
    {
        /// <summary>
        /// Called when this state becomes active.
        /// If <paramref name="cancellationToken"/> is triggered, the transition is aborted.
        /// </summary>
        UniTask Enter(CancellationToken cancellationToken);

        /// <summary>
        /// Called every frame while this state is active and the game is not paused.
        /// </summary>
        void Tick();

        /// <summary>
        /// Called when this state is exited.
        /// If <paramref name="cancellationToken"/> is triggered, cleanup should still run if possible.
        /// </summary>
        UniTask Exit(CancellationToken cancellationToken);
    }
}