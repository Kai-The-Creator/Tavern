using System.Threading;
using _Core._Combat;
using _Core._Combat.Services;
using _Core._Global.CameraService;
using _Core._Global.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core.GameEvents.Battle
{
    public class BattleGameManager : MonoBehaviour, IMiniGameManager
    {
        [SerializeField] private BattleConfig config;
        [SerializeField] private Transform cameraPivot;

        private CombatService combat;
        private ICameraService cams;
        private BattleArena arena;
        private GameObject arenaInstance;
        private CancellationTokenSource cts;

        public bool IsGameActive { get; private set; }
        public bool IsPaused { get; private set; }

        private void Awake()
        {
            combat = GetComponentInChildren<CombatService>(true);
            cams = GService.GetService<ICameraService>();
        }

        public async UniTask StartGame()
        {
            if (IsGameActive) return;
            IsGameActive = true;
            cts = new CancellationTokenSource();

            if (config.ArenaPrefab && arenaInstance == null)
            {
                arenaInstance = Instantiate(config.ArenaPrefab, transform);
                arena = arenaInstance.GetComponent<BattleArena>() ?? arenaInstance.AddComponent<BattleArena>();
            }

            if (cameraPivot)
                await cams.MoveCamera(cameraPivot, 1f, cts.Token, "BATTLE_CAM");

            if (combat)
                await combat.StartBattle(config, arena);
        }

        public async UniTask StopGame()
        {
            if (!IsGameActive) return;
            cts.Cancel();
            combat?.EndBattle();
            if (arenaInstance) Destroy(arenaInstance);
            arenaInstance = null;
            arena = null;
            await cams.MoveCameraToStartPosition();
            IsGameActive = false;
        }

        public UniTask PauseGame()  { IsPaused = true; return UniTask.CompletedTask; }
        public UniTask ResumeGame() { IsPaused = false; return UniTask.CompletedTask; }
        public async UniTask RestartGame() { await StopGame(); await StartGame(); }
    }
}