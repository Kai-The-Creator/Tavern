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
        public void Configure(BattleConfig cfg, IList<CombatEntity> list);
        public UniTask StartBattle(System.Threading.CancellationToken token);
    }

    [DependsOn(typeof(ICameraService), typeof(IEquipService))]
    public class CombatService : AService, ICombatService
    {
        [SerializeField] private BattleConfig config;
        [SerializeField] private List<CombatEntity> combatants;

        private int _current;
        private BattleState _state;
        public BattleState State => _state;

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
                await entity.OnTurnStart(config);

                if (entity.GetComponent<StatusController>()?.IsStunned == true)
                {
                    _current = (_current + 1) % combatants.Count;
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
                }

                _state = DetermineBattleState();
                _current = (_current + 1) % combatants.Count;
                await UniTask.Yield();
            }
        }

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
                    if (source.IsPlayer && ability.MaxTargets > 1)
                    {
                        var selector = source.GetComponent<TargetSelectionController>();
                        if (selector != null)
                            return await selector.SelectTargets(enemies, ability.MaxTargets);
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
            bool playerAlive = combatants.Any(c => c.IsPlayer && c.Resources.Health > 0);
            bool enemiesAlive = combatants.Any(c => !c.IsPlayer && c.Resources.Health > 0);

            if (!playerAlive)
                return BattleState.Defeat;
            if (!enemiesAlive)
                return BattleState.Victory;
            return BattleState.PlayerTurn;
        }
    }
}