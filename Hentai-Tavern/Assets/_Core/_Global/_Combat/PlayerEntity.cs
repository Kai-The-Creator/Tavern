using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public class PlayerEntity : CombatEntity
    {
        [SerializeField] private AbilitySO[] abilities;
        private AbilitySelectionPanel _selectionPanel;
        private BattleConfig _config;
        private PotionController _potionController;

        protected override void Awake()
        {
            base.Awake();
            _potionController = GetComponent<PotionController>();
        }

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
            if (_config && Resources.UltimateCharge >= 100f && _config.UltimateAbility)
                if (!IsOnCooldown(_config.UltimateAbility) && CanUse(_config.UltimateAbility))
                    list.Add(_config.UltimateAbility);

            if (_potionController)
                foreach (var a in _potionController.ActiveAbilities)
                    if (!IsOnCooldown(a) && CanUse(a))
                        list.Add(a);

            if (_selectionPanel == null || list.Count == 0)
                return list.FirstOrDefault();
            return await _selectionPanel.ChooseAbility(list, GetCooldown);
        }

        public void SetSelectionPanel(AbilitySelectionPanel panel)
        {
            _selectionPanel = panel;
        }

        public void SetAbilities(IEnumerable<AbilitySO> list)
        {
            abilities = list?.ToArray() ?? System.Array.Empty<AbilitySO>();
        }

    }
}