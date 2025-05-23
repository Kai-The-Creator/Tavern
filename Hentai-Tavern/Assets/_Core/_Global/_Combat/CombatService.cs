using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using _Core._Global.Services;
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

    public class CombatService : AService
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
                    var targets = (await SelectTargets(entity, ability)).ToList();
                    await AbilityExecutor.Execute(entity, targets, ability);

                    if (entity is CombatEntity ce && ability.Cooldown > 0)
                        ce.Cooldowns[ability] = ability.Cooldown;

                    if (ability == config.UltimateAbility)
                        entity.Resources.UltimateCharge = 0f;
                    else if (ability.PhysicalDamage > 0 || ability.MagicalDamage > 0)
                        entity.Resources.UltimateCharge += config.UltChargePerAttack * targets.Count;

                    entity.Resources.Clamp(entity.Stats);
                }

                _state = DetermineBattleState();
                _current = (_current + 1) % combatants.Count;
                await UniTask.Yield();
            }
        }

        private async UniTask<IEnumerable<ICombatEntity>> SelectTargets(CombatEntity source, AbilitySO ability)
        {
            switch (ability.Target)
            {
                case TargetSelector.Self:
                    return new[] { source };
                case TargetSelector.SingleEnemy:
                    var candidates = combatants.Where(c => c.IsPlayer != source.IsPlayer && c.Resources.Health > 0).ToList();
                    if (source is PlayerEntity player)
                    {
                        var chosen = await player.ChooseTarget(candidates);
                        return new[] { (ICombatEntity)chosen };
                    }
                    else
                    {
                        var enemy = candidates.FirstOrDefault();
                        return enemy != null ? new[] { (ICombatEntity)enemy } : new[] { source };
                    }
                case TargetSelector.AllEnemies:
                    return combatants.Where(c => c.IsPlayer != source.IsPlayer && c.Resources.Health > 0).Cast<ICombatEntity>();
                default:
                    return new[] { source };
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