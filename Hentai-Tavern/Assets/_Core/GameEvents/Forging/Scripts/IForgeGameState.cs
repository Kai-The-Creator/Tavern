using Cysharp.Threading.Tasks;

namespace _Core.GameEvents.Forging.States
{
    public interface IForgeGameState
    {
        UniTask Enter();
        void    Update();
        UniTask Exit();
    }
}