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

        public override async UniTask OnStart()
        {
            _state = BattleState.Init;
            await UniTask.Yield();
        }

        public async UniTask StartBattle()
        {
            _current = 0;
            _state = BattleState.PlayerTurn;
            while (_state != BattleState.Victory && _state != BattleState.Defeat)
            {
                var entity = combatants[_current];
                await entity.OnTurnStart(config);

                var ability = await entity.SelectAbility();
                if (ability != null)
                {
                    var targets = SelectTargets(entity, ability);
                    await AbilityExecutor.Execute(entity, targets, ability);
                }

                _current = (_current + 1) % combatants.Count;
                await UniTask.Yield();
            }
        }

        private IEnumerable<ICombatEntity> SelectTargets(CombatEntity source, AbilitySO ability)
        {
            switch (ability.Target)
            {
                case TargetSelector.Self:
                    return new[] { source };
                case TargetSelector.SingleEnemy:
                    var enemy = combatants.FirstOrDefault(c => c.IsPlayer != source.IsPlayer);
                    return enemy != null ? new[] { (ICombatEntity)enemy } : new[] { source };
                case TargetSelector.AllEnemies:
                    return combatants.Where(c => c.IsPlayer != source.IsPlayer).Cast<ICombatEntity>();
                default:
                    return new[] { source };
            }
        }
    }
}