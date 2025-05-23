using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using _Core._Combat.Services;
using _Core._Global.Services;

namespace _Core._Combat.UI
{
    /// <summary>
    /// Simple HUD for battle displaying player resources and ability buttons.
    /// </summary>
    public class BattleHUD : MonoBehaviour
    {
        [Header("Bars")]
        [SerializeField] private Slider hpBar;
        [SerializeField] private Slider manaBar;
        [SerializeField] private Slider staminaBar;
        [Header("Panels")]
        [SerializeField] private AbilitySelectionPanel abilityPanel;
        [SerializeField] private PotionIndicator potionIndicator;

        public AbilitySelectionPanel AbilityPanel => abilityPanel;
        public PotionIndicator PotionIndicator => potionIndicator;

        private PlayerEntity _player;

        private readonly List<Button> _spawnedAbilities = new();

        private ICombatService _combatService;

        private void OnEnable()
        {
            _combatService = GService.GetService<ICombatService>();
            if (_combatService != null)
                _combatService.OnAbilityResolved += UpdateBars;
        }

        private void OnDisable()
        {
            if (_combatService != null)
                _combatService.OnAbilityResolved -= UpdateBars;
        }

        public void BindPlayer(PlayerEntity player)
        {
            _player = player;
            UpdateBars();
            potionIndicator?.UpdateText();
        }

        public void UpdateBars()
        {
            if (_player == null) return;
            hpBar.value = (float)_player.Resources.Health / _player.Stats.MaxHealth;
            manaBar.value = (float)_player.Resources.Mana / _player.Stats.MaxMana;
            staminaBar.value = (float)_player.Resources.Stamina / _player.Stats.MaxStamina;
        }

        public UniTask<AbilitySO> ChooseAbility(IReadOnlyList<AbilitySO> abilities, Func<AbilitySO, int> cooldown)
        {
            return abilityPanel.ChooseAbility(abilities, cooldown);
        }
    }
}
