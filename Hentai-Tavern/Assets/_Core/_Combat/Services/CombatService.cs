using System.Collections.Generic;
using System.Threading;
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
        [SerializeField] private List<CombatEntity> combatants = new List<CombatEntity>();

        private int _current;
        private BattleState _state;

        public override async UniTask OnStart()
        {
            _state = BattleState.Init;
            await UniTask.Yield();
        }

        public void Configure(BattleConfig cfg, List<CombatEntity> fighters)
        {
            config = cfg;
            combatants = fighters;
        }

        public async UniTask StartBattle(CancellationToken token = default)
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
                    // naive target selection
                    ICombatEntity target = entity;
                    if (ability.Target != TargetSelector.Self && combatants.Count > 1)
                        target = combatants[1 - _current];
                    await AbilityExecutor.Execute(entity, target, ability);
                }

                _current = (_current + 1) % combatants.Count;
                await UniTask.Yield();

                if (token.IsCancellationRequested)
                    break;
            }
        }
    }
}
