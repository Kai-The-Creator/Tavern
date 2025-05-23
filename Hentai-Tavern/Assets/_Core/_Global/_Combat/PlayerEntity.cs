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
                if (!IsOnCooldown(a))
                    list.Add(a);
            if (_config && Resources.UltimateCharge >= 100f && _config.UltimateAbility)
                if (!IsOnCooldown(_config.UltimateAbility))
                    list.Add(_config.UltimateAbility);

            if (_potionController)
                foreach (var a in _potionController.ActiveAbilities)
                    if (!IsOnCooldown(a))
                        list.Add(a);

            if (selectionPanel == null || list.Count == 0)
                return list.FirstOrDefault();

            return await selectionPanel.ChooseAbility(list, GetCooldown);
        }

    }
}