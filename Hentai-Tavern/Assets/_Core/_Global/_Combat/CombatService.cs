using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using _Core._Global.Services;
using _Core._Combat;
using _Core._Global.CameraService;
using _Core._Global.Equip;
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

        private int _current;
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
            _current = 0;
            _state = BattleState.PlayerTurn;
            while (_state != BattleState.Victory && _state != BattleState.Defeat && !token.IsCancellationRequested)
            {
                var entity = combatants[_current];
                if (!entity.IsAlive)
                {
                    _current = (_current + 1) % combatants.Count;
                    continue;
                }

                await entity.OnTurnStart(config);

                var status = entity.GetComponent<StatusController>();
                if (status != null && status.SkipNextTurn)
                {
                    status.SkipNextTurn = false;
                    _current = (_current + 1) % combatants.Count;
                    await UniTask.Yield();
                    continue;
                }

                var ability = await entity.SelectAbility();
                if (ability != null)
                {
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
                }

                _state = DetermineBattleState();
                _current = (_current + 1) % combatants.Count;
                await UniTask.Yield();
            }
        }

<<<<<<< HEAD
        private async UniTask RunTurn(CombatEntity entity)
        {
            if (entity == null || !entity.IsAlive)
                return;

            await entity.OnTurnStart(config);

            var status = entity.GetComponent<StatusController>();
            if (status != null && status.SkipNextTurn)
            {
                status.SkipNextTurn = false;
                await UniTask.Yield();
                return;
            }

            var ability = await entity.SelectAbility();
            if (ability == null)
                return;

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
        }

=======
>>>>>>> parent of f5ba74c (Refactor CombatService turn loop)
        private async UniTask<IReadOnlyList<ICombatEntity>> SelectTargets(CombatEntity source, AbilitySO ability)
        {
            switch (ability.Target)
            {
                case TargetSelector.Self:
                    return new ICombatEntity[] { source };
                case TargetSelector.SingleEnemy:
                    var enemies = combatants
                        .Where(c => c.IsPlayer != source.IsPlayer && c.Resources.Health > 0)
                        .ToList();
                    if (source.IsPlayer)
                    {
                        var selector = source.GetComponent<TargetSelectionController>();
                        if (selector != null && enemies.Count > 0)
                        {
                            var limit = ability.MaxTargets > 1 ? ability.MaxTargets : 1;
                            var selected = await selector.SelectTargets(enemies, limit);
                            if (selected != null && selected.Count > 0)
                                return selected;
                        }
                    }

                    var enemy = enemies.FirstOrDefault();
                    return enemy != null ? new ICombatEntity[] { enemy } : new ICombatEntity[] { source };
                case TargetSelector.AllEnemies:
                    return combatants
                        .Where(c => c.IsPlayer != source.IsPlayer && c.Resources.Health > 0)
                        .Cast<ICombatEntity>()
                        .ToList();
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