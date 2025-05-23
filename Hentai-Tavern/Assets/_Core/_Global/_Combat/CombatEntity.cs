using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public interface ICombatEntity
    {
        string Id { get; }
        bool IsPlayer { get; }
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
        private StatusController _statusController;
        private readonly System.Collections.Generic.Dictionary<AbilitySO,int> _cooldowns = new();

        public string Id => id;
        public bool IsPlayer => isPlayer;
        public StatBlock Stats => stats;
        public ResourcePool Resources => resources;
        public Transform Transform => transform;
        public PassiveAbilitySO[] Passives => passives;
        public System.Collections.Generic.Dictionary<AbilitySO,int> Cooldowns => _cooldowns;

        protected virtual void Awake()
        {
            resources.Health = stats.MaxHealth;
            resources.Mana = stats.MaxMana;
            resources.Stamina = stats.MaxStamina;
            _statusController = GetComponent<StatusController>();
        }

        public virtual UniTask OnTurnStart(BattleConfig config)
        {
            var keys = new System.Collections.Generic.List<AbilitySO>(_cooldowns.Keys);
            foreach (var ability in keys)
                if (_cooldowns[ability] > 0)
                    _cooldowns[ability]--;

            resources.Mana += Mathf.FloorToInt(stats.MaxMana * config.GetManaRegenPercent(IsPlayer));
            resources.Stamina += Mathf.FloorToInt(stats.MaxStamina * config.GetStaminaRegenPercent(IsPlayer));
            resources.Clamp(stats);

            _statusController?.TickStartTurn(this);

            if (passives != null)
            {
                foreach (var passive in passives)
                {
                    passive?.OnTurnStart(this);
                }
            }

            return UniTask.CompletedTask;
        }

        public abstract UniTask<AbilitySO> SelectAbility();
    }
}
