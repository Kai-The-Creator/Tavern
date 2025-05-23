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
        [SerializeField] private Potion[] potions;
        private int _potionsUsed;
        private BattleConfig _config;

        public override UniTask OnTurnStart(BattleConfig config)
        {
            _config = config;
            _potionsUsed = 0;
            return base.OnTurnStart(config);
        }

        public override async UniTask<AbilitySO> SelectAbility()
        {
            var list = new List<AbilitySO>(abilities);
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
            switch (p.Type)
            {
                case PotionType.HealHP:
                    Resources.Health += p.Amount;
                    break;
                case PotionType.RestoreMana:
                    Resources.Mana += p.Amount;
                    break;
                case PotionType.RestoreStamina:
                    Resources.Stamina += p.Amount;
                    break;
            }
            _potionsUsed++;
            Resources.Clamp(Stats);
            return true;
        }
    }
}