// _Core/GameEvents/IMiniGameManager.cs
using Cysharp.Threading.Tasks;

namespace _Core.GameEvents
{
    public interface IMiniGameManager
    {
        bool    IsGameActive { get; }
        bool    IsPaused     { get; }
        UniTask StartGame();
        UniTask PauseGame();
        UniTask ResumeGame();
        UniTask StopGame();
        UniTask RestartGame();
    }
}