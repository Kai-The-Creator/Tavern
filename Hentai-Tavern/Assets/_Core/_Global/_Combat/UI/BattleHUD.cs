using System;
using System.Collections.Generic;
using _Core._Combat.Services;
using _Core._Global.Services;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("Potions")]
        [SerializeField] private PotionButton potionButtonPrefab;
        [SerializeField] private Transform potionContainer;
        [SerializeField] private TextMeshProUGUI potionUsesLabel;

        [Header("Ultimate")]
        [SerializeField] private Button ultimateButton;
        [SerializeField] private Slider ultimateSlider;

        [Header("Turn")]
        [SerializeField] private Button endTurnButton;

        [Header("Status")]
        [SerializeField] private StatusIndicator playerStatusIndicator;

        [Header("Enemy UI")]
        [SerializeField] private HealthBar healthBarPrefab;
        [SerializeField] private StatusLine statusLinePrefab;
        [SerializeField] private Transform enemyContainer;

        public AbilitySelectionPanel AbilityPanel => abilityPanel;

        private PlayerEntity _player;
        private PotionController _potionController;

        private readonly List<PotionButton> _spawnedPotions = new();
        private readonly Dictionary<CombatEntity, (HealthBar bar, StatusLine line)> _enemyUi = new();

        private ICombatService _combatService;

        public event Action OnEndTurn;
        
        private void OnEnable()
        {
            _combatService = GService.GetService<ICombatService>();
            if (_combatService != null)
                _combatService.OnAbilityResolved += UpdateBars;

            if (_potionController)
            {
                _potionController.OnUsesChanged += UpdatePotionUses;
                UpdatePotionUses(_potionController.RemainingUses);
            }
            
            if (endTurnButton)
                endTurnButton.onClick.AddListener(RaiseEndTurn);
        }
        
        private void RaiseEndTurn()
        {
            OnEndTurn?.Invoke();
        }

        private void OnDisable()
        {
            if (_combatService != null)
                _combatService.OnAbilityResolved -= UpdateBars;

            if (_potionController)
                _potionController.OnUsesChanged -= UpdatePotionUses;

            ClearEnemies();
        }

        public void BindPlayer(PlayerEntity player)
        {
            _player = player;
            _potionController = _player ? _player.GetComponent<PotionController>() : null;
            if (playerStatusIndicator)
            {
                playerStatusIndicator.SetPassives(player.Passives);
                player.GetComponent<StatusController>()?.SetIndicator(playerStatusIndicator);
            }
            UpdateBars();
            UpdateUltimate();
            BindPotions();
        }

        public void UpdateBars()
        {
            if (_player == null) return;
            hpBar.value = (float)_player.Resources.Health / _player.Stats.MaxHealth;
            manaBar.value = (float)_player.Resources.Mana / _player.Stats.MaxMana;
            staminaBar.value = (float)_player.Resources.Stamina / _player.Stats.MaxStamina;
            UpdateUltimate();
        }

        public async UniTask<AbilitySO> ChooseAbility(IReadOnlyList<AbilitySO> abilities,
            Func<AbilitySO, int> cooldown)
        {
            var tcs = new UniTaskCompletionSource<AbilitySO>();

            // --- UI-настройка -------------------------------------------------------
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
            var potions = _potionController;
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
                UpdatePotionUses(potions.RemainingUses);
            }
            // -----------------------------------------------------------------------

            var abilityTask = abilityPanel.ChooseAbility(abilities, cooldown);

            // ждём, кто завершится первым
            var (winnerIndex, abilityResult, tcsResult) =
                await UniTask.WhenAny(abilityTask, tcs.Task);

            // housekeeping
            if (ultimateButton) ultimateButton.onClick.RemoveAllListeners();
            ClearPotions();

            // возвращаем выбранную способность
            return winnerIndex == 0 ? abilityResult : tcsResult;
        }

        private void BindPotions()
        {
            if (_potionController)
            {
                _potionController.OnUsesChanged += UpdatePotionUses;
                UpdatePotionUses(_potionController.RemainingUses);
            }
        }

        private void UpdatePotionUses(int count)
        {
            if (potionUsesLabel)
                potionUsesLabel.text = count.ToString();

            var interactable = count > 0;
            foreach (var btn in _spawnedPotions)
            {
                if (btn)
                    btn.Button.interactable = interactable;
            }
        }

        private void ClearPotions()
        {
            foreach (var p in _spawnedPotions)
                if (p) Destroy(p.gameObject);
            _spawnedPotions.Clear();
        }

        public void BindEnemy(CombatEntity enemy)
        {
            if (enemyContainer == null) return;

            HealthBar bar = null;
            StatusLine line = null;

            if (healthBarPrefab)
            {
                bar = Instantiate(healthBarPrefab, enemyContainer);
                bar.SetTarget(enemy);
            }

            if (statusLinePrefab)
            {
                line = Instantiate(statusLinePrefab, enemyContainer);
                line.Bind(enemy);
            }

            _enemyUi[enemy] = (bar, line);
        }

        public void UnbindEnemy(CombatEntity enemy)
        {
            if (_enemyUi.TryGetValue(enemy, out var ui))
            {
                if (ui.bar) Destroy(ui.bar.gameObject);
                if (ui.line) Destroy(ui.line.gameObject);
                _enemyUi.Remove(enemy);
            }
        }

        public void ClearEnemies()
        {
            foreach (var kv in _enemyUi)
            {
                if (kv.Value.bar) Destroy(kv.Value.bar.gameObject);
                if (kv.Value.line) Destroy(kv.Value.line.gameObject);
            }
            _enemyUi.Clear();
        }

        private void UpdateUltimate()
        {
            if (ultimateSlider && _player != null)
                ultimateSlider.value = _player.Resources.UltimateCharge / 100f;
        }
    }
}