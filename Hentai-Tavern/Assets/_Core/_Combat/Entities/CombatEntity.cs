using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public abstract class CombatEntity : MonoBehaviour, ICombatEntity
    {
    [SerializeField] private string id;
    [SerializeField] private bool isPlayer;
    [SerializeField] private StatBlock stats = new StatBlock();
    [SerializeField] private ResourcePool resources = new ResourcePool();

    public string Id => id;
    public bool IsPlayer => isPlayer;
    public StatBlock Stats => stats;
    public ResourcePool Resources => resources;
    public Transform Transform => transform;

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
            return UniTask.CompletedTask;
        }

        public abstract UniTask<AbilitySO> SelectAbility();
    }
}
