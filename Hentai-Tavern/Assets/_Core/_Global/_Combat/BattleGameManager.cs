using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Core._Combat;
using _Core._Combat.Services;
using _Core._Global.Services;
using _Core.GameEvents;

namespace _Core.GameEvents.Battle
{
    public sealed class BattleGameManager : MonoBehaviour, IMiniGameManager
    {
        [Header("Config")] [SerializeField]
        private BattleConfig config;

        [Header("Prefabs")]
        [SerializeField] private CombatEntity playerPrefab;
        [SerializeField] private List<CombatEntity> enemyPrefabs = new();

        [Header("Spawn points")]
        [SerializeField] private Transform playerSpawn;
        [SerializeField] private List<Transform> enemySpawns = new();

        private readonly List<CombatEntity> spawned = new();
        private CombatService combat;
        private CancellationTokenSource battleCts;

        public bool IsGameActive { get; private set; }
        public bool IsPaused { get; private set; }

        private void Awake()
        {
            combat = GService.GetService<CombatService>();
        }

        public async UniTask StartGame()
        {
            if (IsGameActive) return;
            IsGameActive = true;
            IsPaused = false;

            battleCts = new CancellationTokenSource();

            SpawnCombatants();
            combat.Configure(config, spawned);

            await combat.StartBattle(battleCts.Token);

            ClearCombatants();
            IsGameActive = false;
        }

        public UniTask PauseGame()
        {
            IsPaused = true;
            return UniTask.CompletedTask;
        }

        public UniTask ResumeGame()
        {
            IsPaused = false;
            return UniTask.CompletedTask;
        }

        public async UniTask StopGame()
        {
            if (!IsGameActive) return;

            battleCts.Cancel();
            ClearCombatants();
            IsGameActive = false;
            await UniTask.Yield();
        }

        public UniTask RestartGame() => StopGame().ContinueWith(StartGame);

        private void SpawnCombatants()
        {
            ClearCombatants();
            if (playerPrefab && playerSpawn)
                spawned.Add(Instantiate(playerPrefab, playerSpawn.position, playerSpawn.rotation));

            for (int i = 0; i < enemyPrefabs.Count && i < enemySpawns.Count; i++)
            {
                var prefab = enemyPrefabs[i];
                var spawn = enemySpawns[i];
                if (prefab && spawn)
                    spawned.Add(Instantiate(prefab, spawn.position, spawn.rotation));
            }
        }

        private void ClearCombatants()
        {
            foreach (var e in spawned)
                if (e) Destroy(e.gameObject);
            spawned.Clear();
        }
    }
}