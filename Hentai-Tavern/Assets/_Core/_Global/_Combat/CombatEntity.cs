using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public interface ICombatEntity
    {
        string Id { get; }
        bool IsPlayer { get; }
        bool IsAlive { get; }
        StatBlock Stats { get; }
        ResourcePool Resources { get; }
        UniTask OnTurnStart(BattleConfig config);
        UniTask<AbilitySO> SelectAbility();
        Transform Transform { get; }
    }

    public abstract class CombatEntity : MonoBehaviour, ICombatEntity
    {
        [SerializeField] private string id;
        [SerializeField] private bool isPlayer;
        [SerializeField] private StatBlock stats = new StatBlock();
        [SerializeField] private ResourcePool resources = new ResourcePool();
        [SerializeField] private PassiveAbilitySO[] passives;
        protected StatusController _statusController;
        protected PotionController _potionController;
        protected AbilityCooldownController _cooldownController;

        public string Id => id;
        public bool IsPlayer => isPlayer;
        public bool IsAlive => resources.Health > 0;
        public StatBlock Stats => stats;
        public ResourcePool Resources => resources;
        public Transform Transform => transform;
        public PassiveAbilitySO[] Passives => passives;
        public AbilityCooldownController Cooldowns => _cooldownController;

        public int GetCooldown(AbilitySO ability) => _cooldownController ? _cooldownController.GetRemaining(ability) : 0;
        public bool IsOnCooldown(AbilitySO ability) => _cooldownController && _cooldownController.IsOnCooldown(ability);
        public void StartCooldown(AbilitySO ability) => _cooldownController?.StartCooldown(ability);

        protected virtual void Awake()
        {
            resources.Health = stats.MaxHealth;
            resources.Mana = stats.MaxMana;
            resources.Stamina = stats.MaxStamina;
            _statusController = GetComponent<StatusController>();
            _potionController = GetComponent<PotionController>();
            _cooldownController = GetComponent<AbilityCooldownController>();
        }

        public virtual UniTask OnTurnStart(BattleConfig config)
        {
            resources.Mana += Mathf.FloorToInt(stats.MaxMana * config.GetManaRegenPercent(IsPlayer));
            resources.Stamina += Mathf.FloorToInt(stats.MaxStamina * config.GetStaminaRegenPercent(IsPlayer));
            resources.Clamp(stats);

            _statusController?.TickStartTurn(this);
            _cooldownController?.Tick();

            if (passives != null)
            {
                foreach (var passive in passives)
                {
                    passive?.OnTurnStart(this);
                }
            }

            _potionController?.OnTurnStart(config);

            return UniTask.CompletedTask;
        }

        public bool CanUse(AbilitySO ability)
        {
            if (ability == null)
                return false;

            return resources.Mana >= ability.CostMana &&
                   resources.Stamina >= ability.CostStamina;
        }

        public abstract UniTask<AbilitySO> SelectAbility();
    }
}
