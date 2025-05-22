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

        public string Id => id;
        public bool IsPlayer => isPlayer;
        public StatBlock Stats => stats;
        public ResourcePool Resources => resources;
        public Transform Transform => transform;
        public PassiveAbilitySO[] Passives => passives;

        protected virtual void Awake()
        {
            resources.Health = stats.MaxHealth;
            resources.Mana = stats.MaxMana;
            resources.Stamina = stats.MaxStamina;
        }

        public virtual UniTask OnTurnStart(BattleConfig config)
        {
            resources.Mana += Mathf.FloorToInt(stats.MaxMana * config.GetManaRegenPercent(IsPlayer));
            resources.Stamina += Mathf.FloorToInt(stats.MaxStamina * config.GetStaminaRegenPercent(IsPlayer));
            resources.Clamp(stats);

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
