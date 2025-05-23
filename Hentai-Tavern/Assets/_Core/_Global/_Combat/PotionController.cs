using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Core._Global.ItemSystem;
using _Core._Global.Services;

namespace _Core._Combat
{
    public class PotionController : MonoBehaviour, IAbilityProvider
    {
        [SerializeField] private List<PotionAbilitySO> potions = new();

        private IItemService _items;
        private BattleConfig _config;
        private int _usedThisTurn;
        private PotionIndicator _indicator;

        public IReadOnlyList<AbilitySO> ActiveAbilities =>
            potions
                .Where(p => (_items == null || (_items.IsUnlocked(p.Config))) &&
                              _usedThisTurn < (_config ? _config.PotionsPerTurn : 0))
                .Cast<AbilitySO>()
                .ToList();

        public IReadOnlyList<PassiveAbilitySO> PassiveAbilities => System.Array.Empty<PassiveAbilitySO>();

        private void Awake()
        {
            _items = GService.GetService<IItemService>();
            _indicator = GetComponentInChildren<PotionIndicator>();
        }

        public void OnTurnStart(BattleConfig config)
        {
            _config = config;
            _usedThisTurn = 0;
            _indicator?.UpdateText();
        }

        public bool CanUsePotion() => _usedThisTurn < (_config ? _config.PotionsPerTurn : 0);

        public void RegisterUse()
        {
            _usedThisTurn++;
            _indicator?.UpdateText();
        }

        public int RemainingUses => (_config ? _config.PotionsPerTurn : 0) - _usedThisTurn;
    }
}
