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
        [SerializeField] private List<BehaviourPatternSO> enemyBehaviours = new();
        [SerializeField] private AbilitySelectionPanel selectionPanelPrefab;

        [Header("Spawn points")]
        [SerializeField] private Transform playerSpawn;
        [SerializeField] private List<Transform> enemySpawns = new();

        private readonly List<CombatEntity> spawned = new();
        private AbilitySelectionPanel activePanel;
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
@@ -56,45 +59,67 @@ namespace _Core.GameEvents.Battle
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
            PlayerEntity player = null;
            if (playerPrefab && playerSpawn)
            {
                player = Instantiate(playerPrefab, playerSpawn.position, playerSpawn.rotation) as PlayerEntity;
                if (player != null)
                {
                    spawned.Add(player);
                    if (selectionPanelPrefab)
                    {
                        activePanel = Instantiate(selectionPanelPrefab);
                        player.SetSelectionPanel(activePanel);
                    }
                }
            }

            for (int i = 0; i < enemyPrefabs.Count && i < enemySpawns.Count; i++)
            {
                var prefab = enemyPrefabs[i];
                var spawn = enemySpawns[i];
                if (prefab && spawn)
                {
                    var enemy = Instantiate(prefab, spawn.position, spawn.rotation) as EnemyEntity;
                    if (enemy && i < enemyBehaviours.Count && enemyBehaviours[i])
                        enemy.SetBehaviour(enemyBehaviours[i]);
                    spawned.Add(enemy);
                }
            }
        }

        private void ClearCombatants()
        {
            foreach (var e in spawned)
                if (e) Destroy(e.gameObject);
            spawned.Clear();
            if (activePanel)
            {
                Destroy(activePanel.gameObject);
                activePanel = null;
            }
        }
    }
}