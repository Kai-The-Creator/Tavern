using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

        [Header("Status")]
        [SerializeField] private StatusIndicator statusIndicator;

        [Header("Potions")]
        [SerializeField] private PotionButton potionButtonPrefab;
        [SerializeField] private Transform potionContainer;
        [SerializeField] private TextMeshProUGUI potionUsesLabel;

        [Header("Ultimate")]
        [SerializeField] private Button ultimateButton;
        [SerializeField] private Slider ultimateSlider;

        public AbilitySelectionPanel AbilityPanel => abilityPanel;

        private PlayerEntity _player;

        private readonly List<PotionButton> _spawnedPotions = new();

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

            var potions = _player?.GetComponent<PotionController>();
            if (potions)
                potions.OnUsesChanged -= UpdatePotionUses;

            var controller = _player?.GetComponent<StatusController>();
            if (controller && controller.Indicator == statusIndicator)
                controller.SetIndicator(null);
        }

        public void BindPlayer(PlayerEntity player)
        {
            _player = player;
            UpdateBars();
            UpdateUltimate();
            BindPotions();
            if (statusIndicator && _player)
            {
                var controller = _player.GetComponent<StatusController>();
                if (controller)
                    controller.SetIndicator(statusIndicator);
                statusIndicator.SetPassives(_player.Passives);
            }
        }

        public void UpdateBars()
        {
            if (_player == null) return;
            hpBar.value = (float)_player.Resources.Health / _player.Stats.MaxHealth;
            manaBar.value = (float)_player.Resources.Mana / _player.Stats.MaxMana;
            staminaBar.value = (float)_player.Resources.Stamina / _player.Stats.MaxStamina;
            UpdateUltimate();
        }

        public async UniTask<AbilitySO> ChooseAbility(IReadOnlyList<AbilitySO> abilities, Func<AbilitySO, int> cooldown)
        {
            var tcs = new UniTaskCompletionSource<AbilitySO>();

            if (ultimateButton)
            {
                ultimateButton.onClick.RemoveAllListeners();
                if (_player != null && _player.CanUseUltimate())
                {
                    ultimateButton.interactable = true;
                    ultimateButton.onClick.AddListener(() => tcs.TrySetResult(_player.UltimateAbility));
                }
                else
                {
                    ultimateButton.interactable = false;
                }
            }

            ClearPotions();
            var potions = _player?.GetComponent<PotionController>();
            if (potions && potionButtonPrefab && potionContainer)
            {
                foreach (var p in potions.ActiveAbilities)
                {
                    if (p is not PotionAbilitySO potion) continue;
                    var btn = Instantiate(potionButtonPrefab, potionContainer);
                    btn.Setup(potion);
                    btn.Button.onClick.AddListener(() => tcs.TrySetResult(potion));
                    _spawnedPotions.Add(btn);
                }
            }

            var abilityTask = abilityPanel.ChooseAbility(abilities, cooldown);
            var index = await UniTask.WhenAny(abilityTask, tcs.Task);

            if (ultimateButton) ultimateButton.onClick.RemoveAllListeners();
            ClearPotions();

            return index == 0 ? await abilityTask : await tcs.Task;
        }

        private void BindPotions()
        {
            var potions = _player?.GetComponent<PotionController>();
            if (potions)
            {
                potions.OnUsesChanged += UpdatePotionUses;
                UpdatePotionUses(potions.RemainingUses);
            }
        }

        private void UpdatePotionUses(int count)
        {
            if (potionUsesLabel) potionUsesLabel.text = count.ToString();
        }

        private void ClearPotions()
        {
            foreach (var p in _spawnedPotions)
                if (p) Destroy(p.gameObject);
            _spawnedPotions.Clear();
        }

        private void UpdateUltimate()
        {
            if (ultimateSlider && _player != null)
                ultimateSlider.value = _player.Resources.UltimateCharge / 100f;
        }
    }
}