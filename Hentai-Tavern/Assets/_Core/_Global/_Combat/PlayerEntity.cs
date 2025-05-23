using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public class PlayerEntity : CombatEntity
    {
        [SerializeField] private AbilitySO[] abilities;
        [SerializeField] private AbilitySelectionPanel selectionPanel;
        [SerializeField] private TargetSelectionPanel targetPanel;
        [SerializeField] private PotionAbilitySO[] potions;
        private int _potionsUsed;
        private BattleConfig _config;

        public async UniTask<CombatEntity> ChooseTarget(IReadOnlyList<CombatEntity> targets)
        {
            if (targetPanel == null)
                return targets.FirstOrDefault(t => !t.IsPlayer && t.Resources.Health > 0);
            return await targetPanel.ChooseTarget(targets);
        }

        public override UniTask OnTurnStart(BattleConfig config)
        {
            _config = config;
            _potionsUsed = 0;
            return base.OnTurnStart(config);
        }

        public override async UniTask<AbilitySO> SelectAbility()
        {
            var list = new List<AbilitySO>();
            foreach (var ab in abilities)
            {
                if (!Cooldowns.TryGetValue(ab, out var cd) || cd <= 0)
                    list.Add(ab);
            }
            if (_config && Resources.UltimateCharge >= 100f && _config.UltimateAbility)
                list.Add(_config.UltimateAbility);

            if (selectionPanel == null || list.Count == 0)
                return list.FirstOrDefault();

            return await selectionPanel.ChooseAbility(list);
        }

        public bool UsePotion(int index)
        {
            if (potions == null || index < 0 || index >= potions.Length) return false;
            if (_potionsUsed >= _config.PotionsPerTurn) return false;

            var p = potions[index];
            Resources.Health += p.HealAmount;
            Resources.Mana += p.RestoreMana;
            Resources.Stamina += p.RestoreStamina;
            _potionsUsed++;
            Resources.Clamp(Stats);
            return true;
        }
    }
}