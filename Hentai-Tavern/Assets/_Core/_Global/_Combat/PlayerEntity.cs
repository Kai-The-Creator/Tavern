using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Core._Combat.UI;

namespace _Core._Combat
{
    public class PlayerEntity : CombatEntity
    {
        [SerializeField] private AbilitySO[] abilities;
        [SerializeField] private AbilitySelectionPanel selectionPanel; // legacy
        [SerializeField] private BattleHUD hud;
        public BattleHUD Hud { get => hud; set => hud = value; }
        private BattleConfig _config;
        private PotionController _potionController;

        protected override void Awake()
        {
            base.Awake();
            _potionController = GetComponent<PotionController>();
        }

        public override async UniTask OnTurnStart(BattleConfig config)
        {
            _config = config;
            hud?.BindPlayer(this);
            await base.OnTurnStart(config);
            hud?.UpdateBars();
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

            var panel = hud ? hud.AbilityPanel : selectionPanel;
            if (panel == null || list.Count == 0)
                return list.FirstOrDefault();

            return await panel.ChooseAbility(list, GetCooldown);
        }

    }
}