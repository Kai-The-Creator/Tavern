using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public class PlayerEntity : CombatEntity
    {
        [SerializeField] private AbilitySO[] abilities;
        private BattleHUD _hud;
        private BattleConfig _config;

        public override UniTask OnTurnStart(BattleConfig config)
        {
            _config = config;
            return base.OnTurnStart(config);
        }

        public override async UniTask<AbilitySO> SelectAbility()
        {
            var list = new List<AbilitySO>();
            foreach (var a in abilities)
                if (!IsOnCooldown(a) && CanUse(a))
                    list.Add(a);
            if (_hud == null || list.Count == 0)
                return list.FirstOrDefault();
            return await _hud.ChooseAbility(list, GetCooldown);
        }

        public bool CanUseUltimate()
        {
            var ult = UltimateAbility;
            return ult != null && Resources.UltimateCharge >= 100f && !IsOnCooldown(ult) && CanUse(ult);
        }

        public AbilitySO UltimateAbility => _config ? _config.UltimateAbility : null;

        public void SetHUD(BattleHUD hud)
        {
            _hud = hud;
        }

        public void SetAbilities(IEnumerable<AbilitySO> list)
        {
            abilities = list?.ToArray() ?? System.Array.Empty<AbilitySO>();
        }

        public UniTask WaitEndTurn()
        {
            return _hud != null ? _hud.WaitEndTurn() : UniTask.CompletedTask;
        }

    }
}