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
            var list = new List<AbilitySO>(abilities);
            if (_config && Resources.UltimateCharge >= 100f && _config.UltimateAbility)
                list.Add(_config.UltimateAbility);

            if (_potionController)
                list.AddRange(_potionController.ActiveAbilities);

            if (selectionPanel == null || list.Count == 0)
                return list.FirstOrDefault();

            return await selectionPanel.ChooseAbility(list);
        }

    }
}