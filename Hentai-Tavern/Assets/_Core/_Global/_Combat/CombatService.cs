using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using _Core._Global.Services;
using _Core._Combat;
using _Core._Global.CameraService;
using _Core._Global.Equip;
using _Core._Combat.UI;
using UnityEngine;

namespace _Core._Combat.Services
{
    public enum BattleState
    {
        Init,
        PlayerTurn,
        EnemyTurn,
        CheckEnd,
        Victory,
        Defeat
    }

    public interface ICombatService : IService
    {
        event System.Action OnAbilityResolved;

        void Configure(BattleConfig cfg, IList<CombatEntity> list);
        UniTask StartBattle(System.Threading.CancellationToken token);
    }

    [DependsOn(typeof(ICameraService), typeof(IEquipService))]
    public class CombatService : AService, ICombatService
    {
        [SerializeField] private BattleConfig config;
        [SerializeField] private List<CombatEntity> combatants;

        private BattleState _state;
        public BattleState State => _state;

        public event System.Action OnAbilityResolved;

        private void RaiseAbilityResolved() => OnAbilityResolved?.Invoke();

        public override async UniTask OnStart()
        {
            _state = BattleState.Init;
            await UniTask.Yield();
        }

        public void Configure(BattleConfig cfg, IList<CombatEntity> list)
        {
            config = cfg;
            combatants = list.ToList();
        }

        public async UniTask StartBattle(System.Threading.CancellationToken token)
        {
            if (combatants == null || combatants.Count == 0)
                return;
            _state = DetermineBattleState();

            var hud = Object.FindObjectOfType<BattleHUD>();
            bool endTurn = false;
            void EndTurnHandler() => endTurn = true;
            if (hud) hud.OnEndTurn += EndTurnHandler;

            try
            {
                while (_state != BattleState.Victory && _state != BattleState.Defeat && !token.IsCancellationRequested)
                {
                    // player phase
                    var player = combatants.FirstOrDefault(c => c.IsPlayer);
                    if (player == null)
                        break;

                    endTurn = false;
                    while (!endTurn && _state != BattleState.Victory && _state != BattleState.Defeat && !token.IsCancellationRequested)
                    {
                        await RunTurn(player);
                        _state = DetermineBattleState();
                        if (_state == BattleState.Victory || _state == BattleState.Defeat)
                            break;
                    }

                    if (_state == BattleState.Victory || _state == BattleState.Defeat || token.IsCancellationRequested)
                        break;

                    // enemy phase
                    foreach (var enemy in combatants.Where(c => !c.IsPlayer && c.IsAlive))
                    {
                        await RunTurn(enemy);
                        _state = DetermineBattleState();
                        if (_state == BattleState.Victory || _state == BattleState.Defeat || token.IsCancellationRequested)
                            break;
                    }
                }
            }
            finally
            {
                if (hud) hud.OnEndTurn -= EndTurnHandler;
            }
        }

        private async UniTask<AbilitySO> RunTurn(CombatEntity entity)
        {
            if (entity == null || !entity.IsAlive)
                return null;

            await entity.OnTurnStart(config);

            var status = entity.GetComponent<StatusController>();
            if (status != null && status.SkipNextTurn)
            {
                status.SkipNextTurn = false;
                await UniTask.Yield();
                return null;
            }

            var ability = await entity.SelectAbility();
            if (ability == null)
                return null;

            var targets = await SelectTargets(entity, ability);
            await AbilityExecutor.Execute(entity, targets, ability);
            entity.StartCooldown(ability);

            if (ability == config.UltimateAbility)
            {
                entity.Resources.UltimateCharge = 0f;
            }
            else if (ability is PotionAbilitySO)
            {
                entity.GetComponent<PotionController>()?.RegisterUse();
            }
            else if (ability.PhysicalDamage > 0 || ability.MagicalDamage > 0)
            {
                entity.Resources.UltimateCharge += config.UltChargePerAttack * targets.Count;
            }

            entity.Resources.Clamp(entity.Stats);
            RaiseAbilityResolved();

            return ability;
        }

        private async UniTask<IReadOnlyList<ICombatEntity>> SelectTargets(CombatEntity source, AbilitySO ability)
        {
            switch (ability.Target)
            {
                case TargetSelector.Self:
                    return new ICombatEntity[] { source };
                case TargetSelector.SingleEnemy:
                    var enemies = combatants.Where(c => c.IsPlayer != source.IsPlayer && c.IsAlive).ToList();
                    if (source.IsPlayer && ability.MaxTargets > 1)
                    {
                        var selector = source.GetComponent<TargetSelectionController>();
                        if (selector != null)
                            return await selector.SelectTargets(enemies, ability.MaxTargets);
                    }
                    var enemy = enemies.FirstOrDefault();
                    return enemy != null ? new ICombatEntity[] { enemy } : new ICombatEntity[] { source };
                case TargetSelector.AllEnemies:
                    return combatants.Where(c => c.IsPlayer != source.IsPlayer && c.IsAlive).Cast<ICombatEntity>().ToList();
                default:
                    return new ICombatEntity[] { source };
            }
        }

        private BattleState DetermineBattleState()
        {
            bool playerAlive = combatants.Any(c => c.IsPlayer && c.IsAlive);
            bool enemiesAlive = combatants.Any(c => !c.IsPlayer && c.IsAlive);

            if (!playerAlive)
                return BattleState.Defeat;
            if (!enemiesAlive)
                return BattleState.Victory;
            return BattleState.PlayerTurn;
        }
    }
}