using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Core._Combat;
using _Core._Combat.Services;
using _Core._Combat.UI;
using _Core._Global.CameraService;
using _Core._Global.Services;
using _Core.GameEvents;

namespace _Core.GameEvents.Battle
{
    public sealed class BattleGameManager : MonoBehaviour, IMiniGameManager
    {
        [Header("Config")] 
        [SerializeField] private BattleConfig config;

        [Header("Prefabs")]
        [SerializeField] private CombatEntity playerPrefab;
        [SerializeField] private List<CombatEntity> enemyPrefabs = new();
        [SerializeField] private List<BehaviourPatternSO> enemyBehaviours = new();
        [SerializeField] private BattleHUD hud;

        [Header("Spawn points")]
        [SerializeField] private Transform playerSpawn;
        [SerializeField] private List<Transform> enemySpawns = new();
        [SerializeField] private Transform cameraPosition;
        
        private readonly List<CombatEntity> spawned = new();
        private ICameraService cameraService;
        private ICombatService combat;
        private CancellationTokenSource battleCts;

        public bool IsGameActive { get; private set; }
        public bool IsPaused { get; private set; }

        private void Awake()
        {
            cameraService = GService.GetService<ICameraService>();
            combat = GService.GetService<ICombatService>();
        }

        public async UniTask StartGame()
        {
            if (IsGameActive) return;
            IsGameActive = true;
            IsPaused = false;

            battleCts = new CancellationTokenSource();

            await CameraToStart(battleCts.Token);
            
            SpawnCombatants();
            combat.Configure(config, spawned);

            await combat.StartBattle(battleCts.Token);

            await StopGame();
        }
        
        public UniTask CameraToStart(CancellationToken t) =>
            cameraPosition ? cameraService.MoveCamera(cameraPosition, 1f, t, "DefaultTween") : UniTask.CompletedTask;

        public UniTask PauseGame()
        {
            throw new System.NotImplementedException();
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

            var evtManager = GService.GetService<EventsService>();
            if (evtManager && evtManager.IsEventPlaying)
            {
                evtManager.CompleteCurrentEvent();
            }
        }

        public UniTask RestartGame() => StopGame().ContinueWith(StartGame);

        private void SpawnCombatants()
        {
            ClearCombatants();
            PlayerEntity player = null;
            if (playerPrefab && playerSpawn)
            {
                player = Instantiate(playerPrefab, playerSpawn.position, playerSpawn.rotation, playerSpawn) as PlayerEntity;
                if (player != null)
                {
                    spawned.Add(player);
                    hud.BindPlayer(player);
                    player.SetHUD(hud);
                }
            }

            for (int i = 0; i < enemyPrefabs.Count && i < enemySpawns.Count; i++)
            {
                var prefab = enemyPrefabs[i];
                var spawn = enemySpawns[i];
                if (prefab && spawn)
                {
                    var enemy = Instantiate(prefab, spawn.position, spawn.rotation, spawn) as EnemyEntity;
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
        }
    }
}